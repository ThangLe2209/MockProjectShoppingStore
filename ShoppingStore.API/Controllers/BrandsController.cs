using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Serialization;
using ShoppingStore.API.Attributes;
using ShoppingStore.API.Services;
using ShoppingStore.Model;
using ShoppingStore.Model.Dtos;
using System.Collections.Generic;
using System.Text.Json;

namespace ShoppingStore.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BrandsController : Controller
    {
        private readonly IResponseCacheService _responseCacheService;
        private readonly IBrandRepository _brandRepository;
        private readonly IMapper _mapper;
		const int maxBrandsPageSize = 4;
        public BrandsController(IBrandRepository brandRepository, IMapper mapper, IResponseCacheService responseCacheService)
        {
            _brandRepository = brandRepository ?? throw new ArgumentNullException(nameof(brandRepository));
            _responseCacheService = responseCacheService ?? throw new ArgumentNullException(nameof(responseCacheService));
            this._mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<BrandDto>>> GetBrands()
        {
            var brandEntities = await _brandRepository.GetBrandsAsync();
            var result = _mapper.Map<IEnumerable<BrandDto>>(brandEntities);
            return Ok(result);
        }

        [HttpGet("getbrandsByOrderDetailsDebug")]  // Test Action (only use to debug postman - check SumBrandQuantity property).
        public async Task<ActionResult<IEnumerable<BrandModelTest>>> GetBrandsByOrderDetailsDebug() // can use Task<ActionResult<IEnumerable<BrandWithoutProductDto>>> to not Include navigation collection Products
        {
            var brandEntities = await _brandRepository.GetBrandsByOrderDetailCalculationAsync();
            //var result = _mapper.Map<IEnumerable<BrandDto>>(brandEntities); // can use IEnumerable<BrandWithoutProductDto> to not Include navigation collection Products
            return Ok(brandEntities);
        }

        [HttpGet("getbrandsByOrderDetailsPaginate")]
        [Cache(43200)]
        public async Task<ActionResult<IEnumerable<BrandDto>>> GetBrandsByOrderDetailsPaginate(
            string? searchTerm = "", int pageNumber = 1, int pageSize = maxBrandsPageSize) // can use Task<ActionResult<IEnumerable<BrandWithoutProductDto>>> to not Include navigation collection Products
        {
            if (pageSize > maxBrandsPageSize)
            {
                pageSize = maxBrandsPageSize;
            }
            var (brandEntities, paginationMetadata) = await _brandRepository.GetBrandsByOrderDetailCalculationPaginateAsync(pageNumber, pageSize);

            Response.Headers.Append("X-Pagination", JsonSerializer.Serialize(paginationMetadata));

            var result = _mapper.Map<IEnumerable<BrandDto>>(brandEntities); // can use IEnumerable<BrandWithoutProductDto> to not Include navigation collection Products
            return Ok(result);
        }

        //      [HttpGet("getbrandsByOrderDetailsPaginate")]
        //[CacheTestAttribute(1000, true)] // set check cache in controller to true first
        //      public async Task<ActionResult<IEnumerable<BrandDto>>> GetBrandsByOrderDetailsPaginate(
        //          string? searchTerm = "", int pageNumber = 1, int pageSize = maxBrandsPageSize) // can use Task<ActionResult<IEnumerable<BrandWithoutProductDto>>> to not Include navigation collection Products
        //      {

        //	string? cacheDataJson = HttpContext.Items["cacheData"]?.ToString();
        //	if (cacheDataJson != null)
        //	{
        //              // check smthg with cache here if want
        //		if (cacheDataJson.Contains($"item2"))
        //		{
        //                  //cacheResponse = Newtonsoft.Json.JsonConvert.SerializeObject(responseData.item1);
        //                  //var cacheData = Newtonsoft.Json.JsonConvert.DeserializeObject<IEnumerable<BrandDto>>(cacheDataJson);
        //                  //var cacheData = JsonSerializer.Deserialize<dynamic>(cacheDataJson);
        //                  var cacheData = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(cacheDataJson);
        //			var data = Newtonsoft.Json.JsonConvert.DeserializeObject<IEnumerable<BrandDto>>($"{cacheData.item1}");
        //			var data3 = data.GetType();
        //		}
        //		return Ok("CheckCacheOnly");
        //	}

        //          if (pageSize > maxBrandsPageSize)
        //          {
        //              pageSize = maxBrandsPageSize;
        //          }
        //          var (brandEntities, paginationMetadata) = await _brandRepository.GetBrandsByOrderDetailCalculationPaginateAsync(pageNumber,pageSize);

        //	Response.Headers.Append("X-Pagination", JsonSerializer.Serialize(paginationMetadata));

        //          var result = _mapper.Map<IEnumerable<BrandDto>>(brandEntities); // can use IEnumerable<BrandWithoutProductDto> to not Include navigation collection Products
        //          return Ok(result);
        //      }



        //    [HttpGet("getbrandsByOrderDetailsPaginate")]
        //    public async Task<ActionResult<(IEnumerable<BrandDto>, string)>> GetBrandsByOrderDetailsPaginate(
        //		string? searchTerm = "", int pageNumber = 1, int pageSize = maxBrandsPageSize) // can use Task<ActionResult<IEnumerable<BrandWithoutProductDto>>> to not Include navigation collection Products
        //    {
        //        if (pageSize > maxBrandsPageSize)
        //        {
        //            pageSize = maxBrandsPageSize;
        //        }
        //        var (brandEntities, paginationMetadata) = await _brandRepository.GetBrandsByOrderDetailCalculationPaginateAsync(pageNumber, pageSize);

        //        Response.Headers.Append("X-Pagination", JsonSerializer.Serialize(paginationMetadata));

        //        var result = _mapper.Map<IEnumerable<BrandDto>>(brandEntities); // can use IEnumerable<BrandWithoutProductDto> to not Include navigation collection Products
        //        return Ok((result, JsonSerializer.Serialize(paginationMetadata)));
        //    }


        [HttpGet("paginate")]
        public async Task<ActionResult<IEnumerable<BrandDto>>> GetBrandsPaginate(
			string? searchTerm, int pageNumber = 1, int pageSize = maxBrandsPageSize) // must use nullable value(string variable here) or default value(bool in api below) in querystring => string should be nullable, bool/int should have default value
        {
            if (pageSize > maxBrandsPageSize)
            {
                pageSize = maxBrandsPageSize;
            }

            var (brandEntities, paginationMetadata) =
                await _brandRepository.GetBrandsAsync(searchTerm, pageNumber, pageSize); // Tuple return

            Response.Headers.Append("X-Pagination", JsonSerializer.Serialize(paginationMetadata));

            // with mapper nuget package - will remove if the field not exist from source file to dest file
            return Ok(_mapper.Map<IEnumerable<BrandDto>>(brandEntities));
        }

        [HttpGet("{brandId}", Name = "GetBrandById")]
		public async Task<IActionResult> GetBrand(Guid brandId)
		{
			var brand = await _brandRepository.GetBrandById(brandId);
			if (brand == null)
			{
				return NotFound("Brand Not Found");
			}

			return Ok(_mapper.Map<BrandDto>(brand));
		}

		[HttpPost]
		public async Task<IActionResult> CreateBrand([FromBody] BrandForCreationDto brand) // because data create is complex type (PointOfInterestForCreationDto) so by using [ApiController] in line 8 system will automatically know this data from body instead we must specify it like this [FromBody] PointOfInterestForCreationDto pointOfInterest
		{
            //var cacheData = await _responseCacheService.GetCacheResponseAsync("/api/brands/getbrandsByOrderDetailsPaginate|pageNumber-1|pageSize-4|searchTerm-");
            await _responseCacheService.RemoveCacheResponseAsync("/api/brands");
            brand.Slug = brand.Name.Replace(" ", "-");
			var checkSlug = await _brandRepository.GetBrandBySlug(brand.Slug);

			if (checkSlug != null)
			{
				return BadRequest("Brand already exist in database");
			}

			var finalBrand = _mapper.Map<BrandModel>(brand);

			_brandRepository.AddBrand(finalBrand);

			await _brandRepository.SaveChangesAsync(); // after this line execute we will have new Id, foregin key data for variable finalPointOfInterest which auto generated from database (can set breakpoint at line 75, 95, 99 to see) and also update to database
			var createdBrandToReturn = _mapper.Map<BrandDto>(finalBrand);
			return CreatedAtRoute("GetBrandById", // Name of Api Get from line 55 - to set location header in postman when we successfully created - location header will be api get in line 24 Ex: view cap1 image in folder 04 
				new
				{
					brandId = createdBrandToReturn.Id,
				} // value API Get line 24 need - Api get specific pointOfInterest
				, createdBrandToReturn); // final Data (include in response body)
		}

		[HttpPut("{brandId}")]
		public async Task<ActionResult> UpdateBrand(Guid brandId, [FromBody] BrandForEditDto updatedBrand)
		{
            updatedBrand.Slug = updatedBrand.Name.Replace(" ", "-");
			BrandModel currentBrand = await _brandRepository.GetBrandById(brandId);
			if (currentBrand == null)
			{
				//_logger.LogInformation($"City with id {cityId} wasn't found when accessing points of interest.");
				return NotFound("Brand not existed");
			}

			var brandWithSlug = await _brandRepository.GetBrandBySlug(updatedBrand.Slug);
			if (brandWithSlug != null && brandWithSlug.Id != brandId)
			{
				return BadRequest("Brand already existed in database");
			}

			_mapper.Map(updatedBrand, currentBrand); // source, dest => use mapper like this will override data from source to dest
			await _brandRepository.SaveChangesAsync();

            await _responseCacheService.RemoveCacheResponseAsync("/api/brands");
            await _responseCacheService.RemoveCacheResponseAsync("/api/products");
            return NoContent();
		}

		[HttpDelete("{brandId}")]
		public async Task<ActionResult> DeleteBrand(Guid brandId)
		{
            try
			{
				BrandModel currentBrand = await _brandRepository.GetBrandById(brandId);
				if (currentBrand == null)
				{
					//_logger.LogInformation($"City with id {cityId} wasn't found when accessing points of interest.");
					return NotFound("Brand not existed");
				}

				_brandRepository.DeleteBrand(currentBrand);
				await _brandRepository.SaveChangesAsync();
                
                await _responseCacheService.RemoveCacheResponseAsync("/api/brands");
                await _responseCacheService.RemoveCacheResponseAsync("/api/products");

                //_mailService.Send("Point of interest deleted.",
                //    $"Point of interest {pointOfInterestEntity.Name} with id {pointOfInterestEntity.Id} was deleted.");

                return NoContent();
			}
			catch (Exception ex)
			{
				if (ex.InnerException.ToString().Contains("FOREIGN KEY constraint") || ex.InnerException.ToString().Contains("foreign key constraint"))
				{
					return BadRequest("Delete Product first!");
				}
				return BadRequest(ex.GetType().Name);
			}
		}
	}
}
