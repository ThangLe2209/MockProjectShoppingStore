using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using ShoppingStore.API.Services;
using ShoppingStore.Model.Dtos;
using ShoppingStore.Model.Entities;

namespace ShoppingStore.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RolesController : Controller
    {
        private readonly IRoleRepository _roleRepository;
        private readonly IMapper _mapper;
        public RolesController(IRoleRepository roleRepository, IMapper mapper)
        {
            _roleRepository = roleRepository ?? throw new ArgumentNullException(nameof(roleRepository));
            this._mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<RoleDto>>> GetRoles()
        {
            var roleEntities = await _roleRepository.GetRolesAsync();
            var result = _mapper.Map<IEnumerable<RoleDto>>(roleEntities);
            return Ok(result);
        }

        [HttpGet("{roleId}", Name = "GetRoleById")]
        public async Task<IActionResult> GetRole(Guid roleId)
        {
            var role = await _roleRepository.GetRoleById(roleId);
            if (role == null)
            {
                return NotFound("Role Not Found");
            }

            return Ok(_mapper.Map<RoleDto>(role));

        }

        [HttpPost]
        public async Task<IActionResult> CreateRole([FromBody] RoleForCreationDto role) // because data create is complex type (PointOfInterestForCreationDto) so by using [ApiController] in line 8 system will automatically know this data from body instead we must specify it like this [FromBody] PointOfInterestForCreationDto pointOfInterest
        {
            var checkRole = await _roleRepository.GetRoleByName(role.Value);

            if (checkRole != null)
            {
                return BadRequest("Role already exist in database");
            }

            var finalRole = _mapper.Map<UserRole>(role);

            _roleRepository.AddRole(finalRole);

            await _roleRepository.SaveChangesAsync(); // after this line execute we will have new Id, foregin key data for variable finalPointOfInterest which auto generated from database (can set breakpoint at line 75, 95, 99 to see) and also update to database
            var createdRoleToReturn = _mapper.Map<RoleDto>(finalRole);
            return CreatedAtRoute("GetRoleById", // Name of Api Get from line 55 - to set location header in postman when we successfully created - location header will be api get in line 24 Ex: view cap1 image in folder 04 
                new
                {
                    roleId = createdRoleToReturn.Id,
                } // value API Get line 24 need - Api get specific pointOfInterest
                , createdRoleToReturn); // final Data (include in response body)
        }

        [HttpPut("{roleId}")]
        public async Task<ActionResult> UpdateRole(Guid roleId, [FromBody] RoleForEditDto updatedRole)
        {
            UserRole currentRole = await _roleRepository.GetRoleById(roleId);
            if (currentRole == null)
            {
                //_logger.LogInformation($"City with id {cityId} wasn't found when accessing points of interest.");
                return NotFound("Role not existed");
            }

            var otherRole = await _roleRepository.GetRoleByName(updatedRole.Value);
            if (otherRole != null && otherRole.Id != roleId)
            {
                return BadRequest("Role already existed in database");
            }

            _mapper.Map(updatedRole, currentRole); // source, dest => use mapper like this will override data from source to dest
            await _roleRepository.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{roleId}")]
        public async Task<ActionResult> DeleteRole(Guid roleId)
        {
            UserRole currentRole = await _roleRepository.GetRoleById(roleId);
            if (currentRole == null)
            {
                //_logger.LogInformation($"City with id {cityId} wasn't found when accessing points of interest.");
                return NotFound("Role not existed");
            }

            _roleRepository.DeleteRole(currentRole);
            await _roleRepository.SaveChangesAsync();

            //_mailService.Send("Point of interest deleted.",
            //    $"Point of interest {pointOfInterestEntity.Name} with id {pointOfInterestEntity.Id} was deleted.");

            return NoContent();
        }
    }
}
