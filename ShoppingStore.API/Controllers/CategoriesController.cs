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
    public class CategoriesController : Controller
    {
        private readonly IResponseCacheService _responseCacheService;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IMapper _mapper;
        const int maxCategoriesPageSize = 20;
        public CategoriesController(ICategoryRepository categoryRepository, IMapper mapper, IResponseCacheService responseCacheService)
        {
            _categoryRepository = categoryRepository ?? throw new ArgumentNullException(nameof(categoryRepository));
            _responseCacheService = responseCacheService ?? throw new ArgumentNullException(nameof(responseCacheService));
            this._mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CategoryDto>>> GetCategories()
        {
            var categoryEntities = await _categoryRepository.GetCategoriesAsync();
            var result = _mapper.Map<IEnumerable<CategoryDto>>(categoryEntities);
            return Ok(result);
        }

		[HttpGet("paginate")]
		public async Task<ActionResult<IEnumerable<CategoryDto>>> GetCategoriesPaginate(
		[FromQuery(Name = "name")] string? name, string? searchQuery, int pageNumber = 1, int pageSize = 10) // must use nullable value(string variable here) or default value(bool in api below) in querystring => string should be nullable, bool/int should have default value
		{
			// List Cities can be empty so dont need check null here
			//return Ok(_citiesDataStore.Cities); //CitiesDataStore.Current.Cities

			if (pageSize > maxCategoriesPageSize)
			{
				pageSize = maxCategoriesPageSize;
			}

			var (cityEntities, paginationMetadata) =
				await _categoryRepository.GetCategoriesAsync(name, searchQuery, pageNumber, pageSize); // Tuple return

			Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(paginationMetadata));

			// with mapper nuget package - will remove if the field not exist from source file to dest file
			return Ok(_mapper.Map<IEnumerable<CategoryDto>>(cityEntities));
		}

		[HttpGet("{categoryId}", Name = "GetCategoryById")]
		public async Task<IActionResult> GetCategory(Guid categoryId)
		{
			var category = await _categoryRepository.GetCategoryById(categoryId);
			if (category == null)
			{
				return NotFound("Category Not Found");
			}

			return Ok(_mapper.Map<CategoryDto>(category));

		}

		[HttpPost]
		public async Task<IActionResult> CreateCategory([FromBody] CategoryForCreationDto category) // because data create is complex type (PointOfInterestForCreationDto) so by using [ApiController] in line 8 system will automatically know this data from body instead we must specify it like this [FromBody] PointOfInterestForCreationDto pointOfInterest
		{
			category.Slug = category.Name.Replace(" ", "-");
			var checkSlug = await _categoryRepository.GetCategoryBySlug(category.Slug);

			if (checkSlug != null)
			{
				return BadRequest("Category already exist in database");
			}

			var finalCategory = _mapper.Map<CategoryModel>(category);

			_categoryRepository.AddCategory(finalCategory);

			await _categoryRepository.SaveChangesAsync(); // after this line execute we will have new Id, foregin key data for variable finalPointOfInterest which auto generated from database (can set breakpoint at line 75, 95, 99 to see) and also update to database
			var createdCategoryToReturn = _mapper.Map<CategoryDto>(finalCategory);
			return CreatedAtRoute("GetCategoryById", // Name of Api Get from line 55 - to set location header in postman when we successfully created - location header will be api get in line 24 Ex: view cap1 image in folder 04 
				new
				{
					categoryId = createdCategoryToReturn.Id,
				} // value API Get line 24 need - Api get specific pointOfInterest
				, createdCategoryToReturn); // final Data (include in response body)
		}

		[HttpPut("{categoryId}")]
		public async Task<ActionResult> UpdateCategory(Guid categoryId, [FromBody] CategoryForEditDto updatedCategory)
		{
			updatedCategory.Slug = updatedCategory.Name.Replace(" ", "-");
			CategoryModel currentCategory = await _categoryRepository.GetCategoryById(categoryId);
			if (currentCategory == null)
			{
				//_logger.LogInformation($"City with id {cityId} wasn't found when accessing points of interest.");
				return NotFound("Category not existed");
			}

			var categoryWithSlug = await _categoryRepository.GetCategoryBySlug(updatedCategory.Slug);
			if (categoryWithSlug != null && categoryWithSlug.Id != categoryId)
			{
				return BadRequest("Category already existed in database");
			}

			_mapper.Map(updatedCategory, currentCategory); // source, dest => use mapper like this will override data from source to dest
			await _categoryRepository.SaveChangesAsync();

            await _responseCacheService.RemoveCacheResponseAsync("/api/products");
            return NoContent();
		}

		[HttpDelete("{categoryId}")]
		public async Task<ActionResult> DeleteCategory(Guid categoryId)
		{
			try
			{
				CategoryModel currentCategory = await _categoryRepository.GetCategoryById(categoryId);
				if (currentCategory == null)
				{
					//_logger.LogInformation($"City with id {cityId} wasn't found when accessing points of interest.");
					return NotFound("Category not existed");
				}

				_categoryRepository.DeleteCategory(currentCategory);
				await _categoryRepository.SaveChangesAsync();

                //_mailService.Send("Point of interest deleted.",
                //    $"Point of interest {pointOfInterestEntity.Name} with id {pointOfInterestEntity.Id} was deleted.");

                await _responseCacheService.RemoveCacheResponseAsync("/api/products");
                return NoContent();
			}
			catch (Exception ex)
			{
				if (ex.InnerException.ToString().Contains("FOREIGN KEY constraint"))
				{
					return BadRequest("Delete Product first!");
				}
				return BadRequest(ex.GetType().Name);
			}
		}
	}
}
