using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using ShoppingStore.API.Services;
using ShoppingStore.Model.Dtos;
using ShoppingStore.Model;

namespace ShoppingStore.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ShippingsController : Controller
    {
        private readonly IShippingRepository _shippingRepository;
        private readonly IMapper _mapper;
        const int maxShippingQuantitiesPageSize = 4;
        public ShippingsController(IShippingRepository shippingRepository, IMapper mapper)
        {
            _shippingRepository = shippingRepository ?? throw new ArgumentNullException(nameof(shippingRepository));
            this._mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ShippingDto>>> GetShippings()
        {
            //var b = Request.Headers; // check cookies
            //var a = 10;
            var shippingEntities = await _shippingRepository.GetShippingsAsync();
            var result = _mapper.Map<IEnumerable<ShippingDto>>(shippingEntities);
            return Ok(result);
        }

        [HttpGet("getByGeoData")]
        public async Task<ActionResult<ShippingDto?>> GetShippingByGeoDataAsync(string City, string District, string Ward)
        {
            //var b = Request.Headers; // check cookies
            //var a = 10;
            var shippingEntity = await _shippingRepository.GetShippingByGeoDataAsync(City, District, Ward);
            if (shippingEntity == null)
            {
                return NotFound("Shipping Not Found");
                //return StatusCode(500, "Test");
            }
            var result = _mapper.Map<ShippingDto?>(shippingEntity);
            return Ok(result);
        }

        [HttpGet("{shippingId}", Name = "GetShippingById")]
        public async Task<IActionResult> GetShippingById(Guid shippingId)
        {
            var shipping = await _shippingRepository.GetShippingById(shippingId);
            if (shipping == null)
            {
                return NotFound("Shipping Not Found");
            }

            return Ok(_mapper.Map<ShippingDto>(shipping));
        }

        [HttpPost]
        public async Task<IActionResult> CreateShipping([FromBody] ShippingForCreationDto shipping) // because data create is complex type (PointOfInterestForCreationDto) so by using [ApiController] in line 8 system will automatically know this data from body instead we must specify it like this [FromBody] PointOfInterestForCreationDto pointOfInterest
        {
            var existingShipping = await _shippingRepository.CheckExistingShipping(shipping);

            if (existingShipping)
            {
                return BadRequest(new { duplicate = true, message = "Duplicate data." });
            }
            var finalShipping = _mapper.Map<ShippingModel>(shipping);

            _shippingRepository.AddShipping(finalShipping);

            await _shippingRepository.SaveChangesAsync(); // after this line execute we will have new Id, foregin key data for variable finalPointOfInterest which auto generated from database (can set breakpoint at line 75, 95, 99 to see) and also update to database
            var createdShippingToReturn = _mapper.Map<ShippingDto>(finalShipping);
            return CreatedAtRoute("GetShippingById", // Name of Api Get from line 55 - to set location header in postman when we successfully created - location header will be api get in line 24 Ex: view cap1 image in folder 04 
                new
                {
                    shippingId = createdShippingToReturn.Id,
                } // value API Get line 24 need - Api get specific pointOfInterest
                , createdShippingToReturn); // final Data (include in response body)
        }

        [HttpDelete("{shippingId}")]
        public async Task<ActionResult> DeleteShipping(Guid shippingId)
        {
            ShippingModel currentShipping = await _shippingRepository.GetShippingById(shippingId);
            if (currentShipping == null)
            {
                //_logger.LogInformation($"City with id {cityId} wasn't found when accessing points of interest.");
                return NotFound("Shipping not existed");
            }

            _shippingRepository.DeleteShipping(currentShipping);
            await _shippingRepository.SaveChangesAsync();

            //_mailService.Send("Point of interest deleted.",
            //    $"Point of interest {pointOfInterestEntity.Name} with id {pointOfInterestEntity.Id} was deleted.");

            return NoContent();
        }
    }
}
