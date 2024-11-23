using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using ShoppingStore.API.Services;
using ShoppingStore.Model;
using ShoppingStore.Model.Dtos;

namespace ShoppingStore.API.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class WishlistsController : Controller
	{
		private readonly IWishlistRepository _wishlistRepository;
		private readonly IMapper _mapper;
		const int maxWishlistsPageSize = 4;
		public WishlistsController(IWishlistRepository wishlistRepository, IMapper mapper)
		{
			_wishlistRepository = wishlistRepository ?? throw new ArgumentNullException(nameof(wishlistRepository));
			this._mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
		}

		[HttpGet("getwithproductanduser/{userSubjectId}")]
		public async Task<ActionResult<IEnumerable<WishlistWithProductAndUserDto>>> GetWishlistWithProductAndUser(Guid userSubjectId) // can use Task<ActionResult<IEnumerable<BrandWithoutProductDto>>> to not Include navigation collection Products
		{
			var wishlistEntities = await _wishlistRepository.GetWishlistWithProductAndUser(userSubjectId);
			var result = _mapper.Map<IEnumerable<WishlistWithProductAndUserDto>>(wishlistEntities); // can use IEnumerable<BrandWithoutProductDto> to not Include navigation collection Products
			return Ok(result);
		}

		[HttpGet("{wishlistId}", Name = "GetWishlistById")]
		public async Task<IActionResult> GetWishlistById(Guid wishlistId)
		{
			var wishlist = await _wishlistRepository.GetWishlistById(wishlistId);
			if (wishlist == null)
			{
				return NotFound("Wishlist Not Found");
			}
			return Ok(_mapper.Map<WishlistDto>(wishlist));
		}

		[HttpPost]
		public async Task<IActionResult> CreateWishlist([FromBody] WishlistForCreationDto wishlist) // because data create is complex type (PointOfInterestForCreationDto) so by using [ApiController] in line 8 system will automatically know this data from body instead we must specify it like this [FromBody] PointOfInterestForCreationDto pointOfInterest
		{
			var checkWishlist = await _wishlistRepository.CheckWishlistByUserIdAndProductId(wishlist.UserId, wishlist.ProductId);
			if (checkWishlist)
			{
				return BadRequest("Wishlist Already Added");
			}
			var finalWishlist = _mapper.Map<WishlistModel>(wishlist);

			_wishlistRepository.AddWishlist(finalWishlist);

			await _wishlistRepository.SaveChangesAsync(); // after this line execute we will have new Id, foregin key data for variable finalPointOfInterest which auto generated from database (can set breakpoint at line 75, 95, 99 to see) and also update to database
			var createdWishlistToReturn = _mapper.Map<WishlistDto>(finalWishlist);
			return CreatedAtRoute("GetWishlistById", // Name of Api Get from line 55 - to set location header in postman when we successfully created - location header will be api get in line 24 Ex: view cap1 image in folder 04 
				new
				{
					wishlistId = createdWishlistToReturn.Id,
				} // value API Get line 24 need - Api get specific pointOfInterest
				, createdWishlistToReturn); // final Data (include in response body)
		}

		[HttpDelete("{wishlistId}")]
		public async Task<ActionResult> DeleteWishlist(Guid wishlistId)
		{
			WishlistModel currentWishlist = await _wishlistRepository.GetWishlistById(wishlistId);
			if (currentWishlist == null)
			{
				//_logger.LogInformation($"City with id {cityId} wasn't found when accessing points of interest.");
				return NotFound("Wishlist not existed");
			}

			_wishlistRepository.DeleteWishlist(currentWishlist);
			await _wishlistRepository.SaveChangesAsync();

			//_mailService.Send("Point of interest deleted.",
			//    $"Point of interest {pointOfInterestEntity.Name} with id {pointOfInterestEntity.Id} was deleted.");

			return NoContent();
		}
	}
}
