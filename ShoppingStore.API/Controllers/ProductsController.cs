using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ShoppingStore.API.Attributes;
using ShoppingStore.API.Services;
using ShoppingStore.Model;
using ShoppingStore.Model.Dtos;
using System.Text.Json;

namespace ShoppingStore.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : Controller
    {
        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IResponseCacheService _responseCacheService;
        const int maxProductsPageSize = 40;

        public ProductsController(IProductRepository productRepository, IMapper mapper, IWebHostEnvironment webHostEnvironment,IResponseCacheService responseCacheService)
        {
            _productRepository = productRepository ?? throw new ArgumentNullException(nameof(productRepository));
            this._mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _webHostEnvironment = webHostEnvironment ?? throw new ArgumentException(nameof(webHostEnvironment));
            _responseCacheService = responseCacheService ?? throw new ArgumentNullException(nameof(responseCacheService));
        }

        [HttpGet]
        //[Authorize(Policy = "ClientApplicationCanWrite")]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetProducts(string? searchTerm)
        {
            var productEntities = await _productRepository.GetProductsAsync(searchTerm);
            var result = _mapper.Map<IEnumerable<ProductDto>>(productEntities);
            return Ok(result);
        }

        [HttpGet("paginate")]
        [Cache(43200)]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetProductsPaginate(
        string? searchTerm, string? sortBy,int? min, int? max, int pageNumber = 1, int pageSize = maxProductsPageSize) // must use nullable value(string variable here) or default value(bool in api below) in querystring => string should be nullable, bool/int should have default value
        {
            if (pageSize > maxProductsPageSize)
            {
                pageSize = maxProductsPageSize;
            }

            var (productEntities, paginationMetadata) =
                await _productRepository.GetProductsAsync(searchTerm, sortBy, min,max, pageNumber, pageSize); // Tuple return

            Response.Headers.Append("X-Pagination", System.Text.Json.JsonSerializer.Serialize(paginationMetadata));

            // with mapper nuget package - will remove if the field not exist from source file to dest file
            return Ok(_mapper.Map<IEnumerable<ProductDto>>(productEntities));
        }

        [HttpGet("productByBrandSlug")]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetProductsByBrandSlug(
        string? slug, string? sortBy, int? min, int? max, int pageNumber = 1, int pageSize = maxProductsPageSize) // must use nullable value(string variable here) or default value(bool in api below) in querystring => string should be nullable, bool/int should have default value
        {
            if (pageSize > maxProductsPageSize)
            {
                pageSize = maxProductsPageSize;
            }

            var (productEntities, paginationMetadata) =
                await _productRepository.GetProductsByBrandSlugAsync(slug, sortBy,min,max, pageNumber, pageSize); // Tuple return

            Response.Headers.Append("X-Pagination", System.Text.Json.JsonSerializer.Serialize(paginationMetadata));

            // with mapper nuget package - will remove if the field not exist from source file to dest file
            return Ok(_mapper.Map<IEnumerable<ProductDto>>(productEntities));
        }

        [HttpGet("productByCategorySlug")]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetProductsByCategorySlug(
        string? slug, string? sortBy, int? min, int? max, int pageNumber = 1, int pageSize = maxProductsPageSize) // must use nullable value(string variable here) or default value(bool in api below) in querystring => string should be nullable, bool/int should have default value
        {
            if (pageSize > maxProductsPageSize)
            {
                pageSize = maxProductsPageSize;
            }

            var (productEntities, paginationMetadata) =
                await _productRepository.GetProductsByCategorySlugAsync(slug, sortBy, min, max, pageNumber, pageSize); // Tuple return

            Response.Headers.Append("X-Pagination", System.Text.Json.JsonSerializer.Serialize(paginationMetadata));

            // with mapper nuget package - will remove if the field not exist from source file to dest file
            return Ok(_mapper.Map<IEnumerable<ProductDto>>(productEntities));
        }

        [HttpGet("{productId}", Name = "GetProductById")]
        public async Task<IActionResult> GetProduct(Guid productId)
        {
            var product = await _productRepository.GetProductAsync(productId);
            if (product == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<ProductDto>(product));
        }

        [HttpPost]
        public async Task<IActionResult> CreateProduct([FromForm] ProductForCreationDto product) // because data create is complex type (PointOfInterestForCreationDto) so by using [ApiController] in line 8 system will automatically know this data from body instead we must specify it like this [FromBody] PointOfInterestForCreationDto pointOfInterest
        {
            var checkSlug = await _productRepository.GetProductBySlugAsync(product.Slug);

            if (checkSlug != null)
            {
                return BadRequest("Product already exist in database");
            }
            await _responseCacheService.RemoveCacheResponseAsync("/api/products");

            var finalProduct = _mapper.Map<ProductModel>(product);

            await _productRepository.AddProductAsync(finalProduct);

            await _productRepository.SaveChangesAsync(); // after this line execute we will have new Id, foregin key data for variable finalPointOfInterest which auto generated from database (can set breakpoint at line 75, 95, 99 to see) and also update to database
            var createdProductToReturn = _mapper.Map<ProductDto>(finalProduct);
            return CreatedAtRoute("GetProductById", // Name of Api Get from line 55 - to set location header in postman when we successfully created - location header will be api get in line 24 Ex: view cap1 image in folder 04 
                new
                {
                    productId = createdProductToReturn.Id,
                } // value API Get line 89 need - Api get specific product by id
                , createdProductToReturn); // final Data (include in response body)
        }

        [HttpPut("{productId}")]
        public async Task<IActionResult> UpdateProduct(Guid productId, [FromForm] ProductForEditDto updatedProduct)
        {
            ProductModel currentProduct = await _productRepository.GetProductAsync(productId);
            if (currentProduct == null)
            {
                //_logger.LogInformation($"City with id {cityId} wasn't found when accessing points of interest.");
                return NotFound("Product not existed");
            }

            var productWithSlug = await _productRepository.GetProductBySlugAsync(updatedProduct.Slug);
            if (productWithSlug != null && productWithSlug.Id != productId)
            {
                return BadRequest("Product already exist in database");
            }
            await _responseCacheService.RemoveCacheResponseAsync("/api/products");

            if (updatedProduct.ImageUpload != null)
            {
                string uploadsDir = Path.Combine(_webHostEnvironment.WebRootPath, "media/products");
                string imageName = Guid.NewGuid().ToString() + "_" + updatedProduct.ImageUpload.FileName;
                string filePath = Path.Combine(uploadsDir, imageName);
                FileStream fs = new FileStream(filePath, FileMode.Create);
                await updatedProduct.ImageUpload.CopyToAsync(fs); // copy image fo file by mode create
                fs.Close();

                //delete old picture
                string oldfilePath = Path.Combine(uploadsDir, currentProduct.Image);

                try
                {
                    if (System.IO.File.Exists(oldfilePath))
                    {
                        System.IO.File.Delete(oldfilePath);
                    }
                    currentProduct.Image = imageName;
                }
                catch (Exception ex)
                {
                    return BadRequest("An error occurred while deleting the product image.");
                }

            }
            _mapper.Map(updatedProduct, currentProduct); // source, dest => use mapper like this will override data from source to dest
            await _productRepository.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{productId}")]
        public async Task<ActionResult> DeleteProduct(Guid productId)
        {
            try
            {

                ProductModel currentProduct = await _productRepository.GetProductAsync(productId);
                var productImagePath = currentProduct?.Image;
                if (currentProduct == null)
                {
                    //_logger.LogInformation($"City with id {cityId} wasn't found when accessing points of interest.");
                    return NotFound("Product not existed");
                }
                await _responseCacheService.RemoveCacheResponseAsync("/api/products");

                _productRepository.DeleteProduct(currentProduct);
                await _productRepository.SaveChangesAsync();
				if (productImagePath != "noimage.jpg")  _productRepository.DeleteProductImage(productImagePath); // leave behind because if can't not delete image success still fine but when delete product catch FK key constraint if we move this line to above then image will delete descpite of Product not

                //_mailService.Send("Point of interest deleted.",
                //    $"Point of interest {pointOfInterestEntity.Name} with id {pointOfInterestEntity.Id} was deleted.");

                return NoContent();
            }
            catch (Exception ex)
            {
                if (ex.InnerException.ToString().Contains("FOREIGN KEY constraint") || ex.InnerException.ToString().Contains("foreign key constraint"))
                {
                    return BadRequest("Delete OrderDetail first!");
                }
                return BadRequest(ex.GetType().Name);
            }
        }

        [HttpPatch("{productId}")]
        public async Task<ActionResult> PartiallyUpdateProductAsync(Guid productId,
            [FromBody] JsonPatchDocument<ProductForEditWithQuantityDto> patchDocument) //// Need to download nuget package: Microsoft.AspNetCore.JsonPatch
        {
            // https://stackoverflow.com/questions/70184265/http-client-patch-method-with-asp-net-core-2-1-bad-request
            // https://github.com/KevinDockx/JsonPatch
            var productEntity = await _productRepository.GetProductAsync(productId);
            if (productEntity == null)
            {
                return NotFound("Product Not Exist!");
            }
            var productToPatch = _mapper.Map<ProductForEditWithQuantityDto>(productEntity);

            patchDocument.ApplyTo(productToPatch, ModelState);
            // ApiController not automatic check this time
            if (!ModelState.IsValid) // check for input formatter - here is JsonPatchDocument => as long as document valid it will true => miss case validation required name if we choose opt remove in documentjsonpatch
            {
                return BadRequest(ModelState);
            }

            // also check pointOfInterestToPatch (dto) after check json patch document (is it still valid after patchDocument has been applied) => now not miss case remove required name, and some other validation case
            if (!TryValidateModel(productToPatch))
            {
                return BadRequest(JsonConvert.SerializeObject(ModelState));
            }

            _mapper.Map(productToPatch, productEntity); //ovveride left - source(Models.PointOfInterestForUpdateDto) to right- dest(Entities.PointOfInterest)
            await _productRepository.SaveChangesAsync();

            return NoContent();
        }

        [HttpPatch("partiallyUpdateProducts")]
        public async Task<ActionResult> PartiallyUpdateProductsAsync(
         [FromBody] Dictionary<Guid, string> patchDocuments) //// Need to download nuget package: Microsoft.AspNetCore.JsonPatch
        {
            foreach (var (productId, jsonPatchDocument) in patchDocuments)
            {
                JsonPatchDocument<ProductForEditWithQuantityDto> patchDocument = 
                    JsonConvert.DeserializeObject<JsonPatchDocument<ProductForEditWithQuantityDto>>(jsonPatchDocument);
                var productEntity = await _productRepository.GetProductAsync(productId);
                //var a = 10;
                if (productEntity == null)
                {
                    return NotFound($"Product with ProductId {productId} Not Exist!");
                }
                //var b = 10;
                var productToPatch = _mapper.Map<ProductForEditWithQuantityDto>(productEntity);
                //var c = 10;
                patchDocument.ApplyTo(productToPatch, ModelState);
                //var d = 10;
                // ApiController not automatic check this time
                if (!ModelState.IsValid) // check for input formatter - here is JsonPatchDocument => as long as document valid it will true => miss case validation required name if we choose opt remove in documentjsonpatch
                {
                    //var e = 10;
                    return BadRequest(JsonConvert.SerializeObject(ModelState));
                }

                // also check pointOfInterestToPatch (dto) after check json patch document (is it still valid after patchDocument has been applied) => now not miss case remove required name, and some other validation case
                if (!TryValidateModel(productToPatch))
                {
                    //var f = 10;
                    return BadRequest(JsonConvert.SerializeObject(ModelState));
                }
                //var g = 10;
                _mapper.Map(productToPatch, productEntity); //ovveride left - source(Models.PointOfInterestForUpdateDto) to right- dest(Entities.PointOfInterest)
                //var h = 10;
            }
            await _productRepository.SaveChangesAsync();

            return NoContent();
        }
    }
}
