using AutoMapper;
using IdentityModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShoppingStore.API.Services;
using ShoppingStore.Model.Dtos;
using ShoppingStore.Model.Entities;
using System.Web;

namespace ShoppingStore.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : Controller
    {
        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IConfiguration Configuration;
        private readonly IMapper _mapper;
        public UsersController(IUserRepository userRepository, IRoleRepository roleRepository, IMapper mapper, IConfiguration configuration)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _roleRepository = roleRepository ?? throw new ArgumentNullException(nameof(roleRepository)); ;
            this._mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            Configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetUsers()
        {
            var userEntities = await _userRepository.GetUsersAsync();
            var result = _mapper.Map<IEnumerable<UserDto>>(userEntities);
            return Ok(result);
        }

        [HttpGet("{userId}", Name = "GetUserById")]
        public async Task<IActionResult> GetUser(Guid userId, [FromQuery] string? type)
        {
            var user = await _userRepository.GetUserById(userId, type);
            if (user == null)
            {
                return NotFound("User Not Found");
            }

            return Ok(_mapper.Map<UserDto>(user));
        }

        [HttpGet("getUserByEmail", Name = "GetUserByEmail")]
        public async Task<IActionResult> GetUserByEmail(string userEmail)
        {
            var user = await _userRepository.GetUserByEmailAsync(userEmail);
            if (user == null)
            {
                return NotFound("User Not Found");
            }

            return Ok(_mapper.Map<UserDto>(user));
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] UserForCreationDto user) // because data create is complex type (PointOfInterestForCreationDto) so by using [ApiController] in line 8 system will automatically know this data from body instead we must specify it like this [FromBody] PointOfInterestForCreationDto pointOfInterest
        {
            // create user & claims
            var userToCreate = new User
            {
                UserName = user.UserName,
                Subject = Guid.NewGuid().ToString(),
                Email = user.Email,
                Active = false,
                UserRoleId = user.UserRoleId
            };

            userToCreate.Claims.Add(new UserClaim()
            {
                Type = JwtClaimTypes.GivenName,
                Value = user.UserName
            });

            userToCreate.Claims.Add(new UserClaim()
            {
                Type = "country",
                Value = user.Country
            });

            var userRole = await _roleRepository.GetRoleById(user.UserRoleId);
            userToCreate.Claims.Add(new UserClaim()
            {
                Type = "role",
                Value = userRole.Value
            });

            userToCreate.Claims.Add(new UserClaim()
            {
                Type = JwtClaimTypes.Email,
                Value = user.Email
            });

            var finalUserResult = _userRepository.AddUser(userToCreate, user.Password);
            if (finalUserResult is string) return BadRequest(finalUserResult);

            await _userRepository.SaveChangesAsync();

            // create an activation link - we need an absolute URL, therefore
            // we use Url.PageLink instead of Url.Page (fake similar click from email service like Gmail,...)
            var activationLink = $"{Configuration["IDPServerRoot"]}user/activation/index?securityCode={HttpUtility.UrlEncode(userToCreate.SecurityCode)}";

            Console.WriteLine($"Activation link: {activationLink}");

            var createdUserToReturn = _mapper.Map<UserDto>(userToCreate);
            return CreatedAtRoute("GetUserById", // Name of Api Get from line 55 - to set location header in postman when we successfully created - location header will be api get in line 24 Ex: view cap1 image in folder 04 
                new
                {
                    userId = createdUserToReturn.Id,
                } // value API Get line 24 need - Api get specific pointOfInterest
                , createdUserToReturn); // final Data (include in response body)
        }

        [HttpPost("activeAccount")]
        public async Task<IActionResult> ActiveAccount([FromBody] string? statusCode)
        {
            var updateUserAccount = await _userRepository.ActivateUserAsync(statusCode);
            await _userRepository.SaveChangesAsync();
            return Ok();
        }

        [HttpPut("{userId}")]
        public async Task<ActionResult> UpdateUser(Guid userId, [FromBody] UserForEditDto updatedUser)
        {
            try
            {
                User currentUser = await _userRepository.GetUserById(userId, null);
                if (currentUser == null)
                {
                    //_logger.LogInformation($"City with id {cityId} wasn't found when accessing points of interest.");
                    return NotFound("User not existed");
                }
                var checkOtherUser = await _userRepository.IsUsernameOrEmailExist(userId, updatedUser.UserName, updatedUser.Email);
                if (checkOtherUser == true)
                {
                    return BadRequest("User already existed in database");
                }

                if (updatedUser.UserRoleId == Guid.Empty)
                {
                    updatedUser.UserRoleId = currentUser.UserRoleId;
                }

                await _userRepository.DeleteUserClaim(userId);
                _mapper.Map(updatedUser, currentUser); // source, dest => use mapper like this will override data from source to dest
                if (updatedUser.Password != null) _userRepository.HashUserPassword(currentUser, updatedUser.Password);

                // Nếu là admin thì ko gửi mail ko phải thì phải gửi mail nữa để xác thực Email (cần làm thêm)

                currentUser.Claims.Add(new UserClaim()
                {
                    Type = JwtClaimTypes.GivenName,
                    Value = updatedUser.UserName
                });

                currentUser.Claims.Add(new UserClaim()
                {
                    Type = "country",
                    Value = updatedUser.Country
                });

                var userRole = await _roleRepository.GetRoleById(updatedUser.UserRoleId);
                currentUser.Claims.Add(new UserClaim()
                {
                    Type = "role",
                    Value = userRole.Value
                });
                currentUser.Claims.Add(new UserClaim()
                {
                    Type = JwtClaimTypes.Email,
                    Value = updatedUser.Email
                });

                currentUser.ConcurrencyStamp = Guid.NewGuid().ToString();
                await _userRepository.SaveChangesAsync();
                return NoContent();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return BadRequest("Someone has just updated data. Please try again later!");
            }
        }

        [HttpDelete("{userId}")]
        public async Task<ActionResult> DeleteUser(Guid userId)
        {
            User currentUser = await _userRepository.GetUserById(userId, null);
            if (currentUser == null)
            {
                //_logger.LogInformation($"City with id {cityId} wasn't found when accessing points of interest.");
                return NotFound("User not existed");
            }

            await _userRepository.DeleteUserClaim(userId);
            _userRepository.DeleteUser(currentUser);
            await _userRepository.SaveChangesAsync();

            //_mailService.Send("Point of interest deleted.",
            //    $"Point of interest {pointOfInterestEntity.Name} with id {pointOfInterestEntity.Id} was deleted.");

            return NoContent();
        }
    }
}
