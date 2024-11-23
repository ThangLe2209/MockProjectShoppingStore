using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using ShoppingStore.API.Services;
using ShoppingStore.Model;
using ShoppingStore.Model.Dtos;

namespace ShoppingStore.API.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class ComparesController : Controller
	{
		private readonly ICompareRepository _compareRepository;
		private readonly IMapper _mapper;
		const int maxComparesPageSize = 4;
		public ComparesController(ICompareRepository compareRepository, IMapper mapper)
		{
			_compareRepository = compareRepository ?? throw new ArgumentNullException(nameof(compareRepository));
			this._mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
		}

		[HttpGet("getwithproductanduser/{userSubjectId}")]
		public async Task<ActionResult<IEnumerable<CompareWithProductAndUserDto>>> GetCompareWithProductAndUser(Guid userSubjectId) // can use Task<ActionResult<IEnumerable<BrandWithoutProductDto>>> to not Include navigation collection Products
		{
			var compareEntities = await _compareRepository.GetCompareWithProductAndUser(userSubjectId);
			var result = _mapper.Map<IEnumerable<CompareWithProductAndUserDto>>(compareEntities); // can use IEnumerable<BrandWithoutProductDto> to not Include navigation collection Products
			return Ok(result);
		}

		[HttpGet("{compareId}", Name = "GetCompareById")]
		public async Task<IActionResult> GetCompareById(Guid compareId)
		{
			var compare = await _compareRepository.GetCompareById(compareId);
			if (compare == null)
			{
				return NotFound("Compare Not Found");
			}
			return Ok(_mapper.Map<CompareDto>(compare));
		}

		[HttpPost]
		public async Task<IActionResult> CreateCompare([FromBody] CompareForCreationDto compare) // because data create is complex type (PointOfInterestForCreationDto) so by using [ApiController] in line 8 system will automatically know this data from body instead we must specify it like this [FromBody] PointOfInterestForCreationDto pointOfInterest
		{
			var checkCompare = await _compareRepository.CheckCompareByUserIdAndProductId(compare.UserId, compare.ProductId);
			if (checkCompare)
			{
				return BadRequest("Compare Already Added");
			}
			var finalCompare = _mapper.Map<CompareModel>(compare);

			_compareRepository.AddCompare(finalCompare);

			await _compareRepository.SaveChangesAsync(); // after this line execute we will have new Id, foregin key data for variable finalPointOfInterest which auto generated from database (can set breakpoint at line 75, 95, 99 to see) and also update to database
			var createdCompareToReturn = _mapper.Map<CompareDto>(finalCompare);
			return CreatedAtRoute("GetCompareById", // Name of Api Get from line 55 - to set location header in postman when we successfully created - location header will be api get in line 24 Ex: view cap1 image in folder 04 
				new
				{
					compareId = createdCompareToReturn.Id,
				} // value API Get line 24 need - Api get specific pointOfInterest
				, createdCompareToReturn); // final Data (include in response body)
		}

        [HttpDelete("{compareId}")]
        public async Task<ActionResult> DeleteCompare(Guid compareId)
        {
            CompareModel currentCompare = await _compareRepository.GetCompareById(compareId);
            if (currentCompare == null)
            {
                //_logger.LogInformation($"City with id {cityId} wasn't found when accessing points of interest.");
                return NotFound("Compare not existed");
            }

            _compareRepository.DeleteCompare(currentCompare);
            await _compareRepository.SaveChangesAsync();

            //_mailService.Send("Point of interest deleted.",
            //    $"Point of interest {pointOfInterestEntity.Name} with id {pointOfInterestEntity.Id} was deleted.");

            return NoContent();
        }
    }
}
