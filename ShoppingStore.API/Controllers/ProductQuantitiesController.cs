using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using ShoppingStore.API.Services;
using ShoppingStore.Model;
using ShoppingStore.Model.Dtos;

namespace ShoppingStore.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductQuantitiesController : Controller
    {
        private readonly IResponseCacheService _responseCacheService;
        private readonly IProductQuantityRepository _productQuantityRepository;
        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;
        const int maxProductQuantitiesPageSize = 4;
        public ProductQuantitiesController(IProductQuantityRepository productQuantityRepository, IProductRepository productRepository, IMapper mapper, IResponseCacheService responseCacheService)
        {
            _productQuantityRepository = productQuantityRepository ?? throw new ArgumentNullException(nameof(productQuantityRepository));
            _productRepository = productRepository ?? throw new ArgumentNullException(nameof(productRepository));
            _responseCacheService = responseCacheService ?? throw new ArgumentNullException(nameof(responseCacheService));
            this._mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        [HttpGet("getWithProductId/{productId}")]
        public async Task<IActionResult> GetProductQuantitiesByProductId(Guid productId)
        {
            var productQuantities = await _productQuantityRepository.GetProductQuantitiesByProductId(productId);
            return Ok(_mapper.Map<IEnumerable<ProductQuantityDto>>(productQuantities));
        }

        [HttpGet("{productQuantityId}", Name = "GetProductQuantityById")]
        public async Task<IActionResult> GetProductQuantityById(Guid productQuantityId)
        {
            var productQuantity = await _productQuantityRepository.GetProductQuantityById(productQuantityId);
            if (productQuantity == null)
            {
                return NotFound("Product Quantity Not Found");
            }

            return Ok(_mapper.Map<ProductQuantityDto>(productQuantity));
        }

        [HttpPost]
        public async Task<IActionResult> CreateProductQuantity([FromBody] ProductQuantityForCreationDto productQuantity) // because data create is complex type (PointOfInterestForCreationDto) so by using [ApiController] in line 8 system will automatically know this data from body instead we must specify it like this [FromBody] PointOfInterestForCreationDto pointOfInterest
        {
            var product = await _productRepository.GetProductAsync(productQuantity.ProductId);

            if (product == null)
            {
                return BadRequest("Product not exist in database");
            }
            product.Quantity += productQuantity.Quantity;
            var finalProductQuantity = _mapper.Map<ProductQuantityModel>(productQuantity);

            _productQuantityRepository.AddProductQuantity(finalProductQuantity);

            await _productQuantityRepository.SaveChangesAsync(); // after this line execute we will have new Id, foregin key data for variable finalPointOfInterest which auto generated from database (can set breakpoint at line 75, 95, 99 to see) and also update to database

            await _responseCacheService.RemoveCacheResponseAsync("/api/products");
            var createdProductQuantityToReturn = _mapper.Map<ProductQuantityDto>(finalProductQuantity);
            return CreatedAtRoute("GetProductQuantityById", // Name of Api Get from line 55 - to set location header in postman when we successfully created - location header will be api get in line 24 Ex: view cap1 image in folder 04 
                new
                {
                    productQuantityId = createdProductQuantityToReturn.Id,
                } // value API Get line 24 need - Api get specific pointOfInterest
                , createdProductQuantityToReturn); // final Data (include in response body)
        }
    }
}
