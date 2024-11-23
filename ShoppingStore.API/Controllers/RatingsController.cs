using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using ShoppingStore.API.Services;
using ShoppingStore.Model;
using ShoppingStore.Model.Dtos;
using System.Text.Json;

namespace ShoppingStore.API.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class RatingsController : Controller
	{
		private readonly IRatingRepository _ratingRepository;
		private readonly IMapper _mapper;
		const int maxRatingsPageSize = 10;

		public RatingsController(IRatingRepository ratingRepository, IMapper mapper)
		{
			_ratingRepository = ratingRepository ?? throw new ArgumentNullException(nameof(ratingRepository));
			this._mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
		}


        [HttpGet("{ratingId}", Name = "GetRatingById")]
        public async Task<IActionResult> GetRating(Guid ratingId)
        {
            var rating = await _ratingRepository.GetRatingByIdAsync(ratingId);
            if (rating == null)
            {
                return NotFound("Rating Not Found");
            }

            return Ok(_mapper.Map<RatingDto>(rating));

        }

        [HttpPost]
        public async Task<ActionResult<RatingDto>> CreateRating([FromBody] RatingForCreationDto rating) // because data create is complex type (PointOfInterestForCreationDto) so by using [ApiController] in line 8 system will automatically know this data from body instead we must specify it like this [FromBody] PointOfInterestForCreationDto pointOfInterest
        {

            var finalRating = _mapper.Map<RatingModel>(rating);
            _ratingRepository.AddRating(finalRating);

            await _ratingRepository.SaveChangesAsync(); // after this line execute we will have new Id, foregin key data for variable finalPointOfInterest which auto generated from database (can set breakpoint at line 75, 95, 99 to see) and also update to database
            var createdRatingToReturn = _mapper.Map<RatingDto>(finalRating);
            return CreatedAtRoute("GetRatingById", // Name of Api Get from line 55 - to set location header in postman when we successfully created - location header will be api get in line 24 Ex: view cap1 image in folder 04 
                new
                {
                    ratingId = createdRatingToReturn.Id,
                } // value API Get line 24 need - Api get specific pointOfInterest
                , createdRatingToReturn); // final Data (include in response body)
        }

		[HttpGet("paginate/{productId}")]
		public async Task<ActionResult<IEnumerable<RatingDto>>> GetRatingsPaginate(
			Guid productId, int pageNumber = 1, int pageSize = maxRatingsPageSize) // must use nullable value(string variable here) or default value(bool in api below) in querystring => string should be nullable, bool/int should have default value
		{
			if (pageSize > maxRatingsPageSize)
			{
				pageSize = maxRatingsPageSize;
			}

			var (ratingEntities, paginationMetadata) =
				await _ratingRepository.GetRatingsByProductIdAsync(productId, pageNumber, pageSize); // Tuple return

			Response.Headers.Append("X-Pagination", JsonSerializer.Serialize(paginationMetadata));

			// with mapper nuget package - will remove if the field not exist from source file to dest file
			return Ok(_mapper.Map<IEnumerable<RatingDto>>(ratingEntities));
		}
	}
}
