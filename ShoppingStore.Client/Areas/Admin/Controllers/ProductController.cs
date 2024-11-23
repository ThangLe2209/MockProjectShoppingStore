using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using ShoppingStore.Client.Repository;
using ShoppingStore.Model;
using ShoppingStore.Model.Dtos;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;

namespace ShoppingStore.Client.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin,PayingUser")]
    //[Authorize]
    public class ProductController : Controller
    {
        private readonly ProductService _productService;
        private readonly CategoryService _categoryService;
        private readonly BrandService _brandService;
        private readonly ProductQuantityService _productQuantityService;
        public ProductController(ProductService productService, CategoryService categoryService, BrandService brandService
            , ProductQuantityService productQuantityService)
        {
            _productService = productService;
            _categoryService = categoryService;
            _brandService = brandService;
            _productQuantityService = productQuantityService;
        }
        public async Task<IActionResult> Index()
        {
            ViewBag.Class = "/Admin";
            return View(await _productService.GetProductsAsync(""));
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            ViewBag.Class = "/Admin";
            ViewBag.Categories = new SelectList(await _categoryService.GetCategoriesAsync(), "Id", "Name");
            ViewBag.Brands = new SelectList(await _brandService.GetBrandsAsync(), "Id", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductForCreationDto product)
        {
            ViewBag.Categories = new SelectList(await _categoryService.GetCategoriesAsync(), "Id", "Name", product.CategoryId); // Compare product.CategoryId with CategoryId in Categories context and return ID,Name
            ViewBag.Brands = new SelectList(await _brandService.GetBrandsAsync(), "Id", "Name", product.BrandId);

            if (ModelState.IsValid)
            {
                //add data to db
                //TempData["success"] = "Model da ok het";
                product.Slug = product.Name.Replace(" ", "-");

                using (var multipartFormContent = new MultipartFormDataContent())
                {
                    multipartFormContent.Add(new StringContent(product.Name), "Name");
                    multipartFormContent.Add(new StringContent(product.Slug), "Slug");
                    multipartFormContent.Add(new StringContent(product.Description ?? ""), "Description");
                    multipartFormContent.Add(new StringContent(product.Price.ToString()), "Price");
                    multipartFormContent.Add(new StringContent(product.CapitalPrice.ToString()), "CapitalPrice");
                    multipartFormContent.Add(new StringContent(product.BrandId.ToString()), "BrandId");
                    multipartFormContent.Add(new StringContent(product.CategoryId.ToString()), "CategoryId");

                    // Add the file
                    if (product.ImageUpload != null)
                    {
                        var fileStreamContent = new StreamContent(product.ImageUpload.OpenReadStream());
                        fileStreamContent.Headers.ContentType = MediaTypeHeaderValue.Parse(product.ImageUpload.ContentType);
                        multipartFormContent.Add(fileStreamContent, name: "ImageUpload", product.ImageUpload.FileName);
                    }
                    using var response = await _productService.CreateProductAsync(multipartFormContent);
                    //response.EnsureSuccessStatusCode();
                    if (response.IsSuccessStatusCode) // can use httpResponseMessage.EnsureSuccessStatusCode(); to treat statusCode as error data response
                    {
                        //var responseContent = await response.Content.ReadAsStringAsync();
                        //Console.WriteLine("UploadFileInModel_WithHttpClientAsync response :" + responseContent);
                        TempData["success"] = "Add Product successfully";
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        var errMsg = JsonConvert.DeserializeObject<dynamic>(response.Content.ReadAsStringAsync().Result);
                        TempData["error"] = errMsg;
                        return View(product);
                    }
                }

            }
            else
            {
                TempData["error"] = "Model co 1 vai thu dang bi loi";
                List<string> errors = new List<string>();
                foreach (var value in ModelState.Values)
                {
                    foreach (var error in value.Errors)
                    {
                        errors.Add(error.ErrorMessage);
                    }
                }
                string errorMessage = string.Join("\n", errors);
                //return BadRequest(errorMessage);
                return View(product);
            }
        }

        public async Task<IActionResult> Edit(Guid Id)
        {
            ViewBag.Class = "/Admin";
            ProductDto product = await _productService.GetProductByIdAsync(Id);
            var jsonParent = JsonConvert.SerializeObject(product);
            ProductForEditDto productEdit = JsonConvert.DeserializeObject<ProductForEditDto>(jsonParent);
            ViewBag.Categories = new SelectList(await _categoryService.GetCategoriesAsync(), "Id", "Name", productEdit.CategoryId); // Compare product.CategoryId with CategoryId in Categories context and return ID,Name
            ViewBag.Brands = new SelectList(await _brandService.GetBrandsAsync(), "Id", "Name", productEdit.BrandId);

            return View(productEdit);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid Id, ProductForEditDto product)
        {
            ViewBag.Categories = new SelectList(await _categoryService.GetCategoriesAsync(), "Id", "Name", product.CategoryId); // Compare product.CategoryId with CategoryId in Categories context and return ID,Name
            ViewBag.Brands = new SelectList(await _brandService.GetBrandsAsync(), "Id", "Name", product.BrandId);

            if (ModelState.IsValid)
            {
                //add data to db
                //TempData["success"] = "Model da ok het";
                product.Slug = product.Name.Replace(" ", "-");
                using (var multipartFormContent = new MultipartFormDataContent())
                {
                    multipartFormContent.Add(new StringContent(product.Name), "Name");
                    multipartFormContent.Add(new StringContent(product.Slug), "Slug");
                    multipartFormContent.Add(new StringContent(product.Image), "Image");
                    multipartFormContent.Add(new StringContent(product.Description ?? ""), "Description");
                    multipartFormContent.Add(new StringContent(product.Price.ToString()), "Price");
                    multipartFormContent.Add(new StringContent(product.CapitalPrice.ToString()), "CapitalPrice");
                    multipartFormContent.Add(new StringContent(product.BrandId.ToString()), "BrandId");
                    multipartFormContent.Add(new StringContent(product.CategoryId.ToString()), "CategoryId");

                    // Add the file
                    if (product.ImageUpload != null)
                    {
                        var fileStreamContent = new StreamContent(product.ImageUpload.OpenReadStream());
                        fileStreamContent.Headers.ContentType = MediaTypeHeaderValue.Parse(product.ImageUpload.ContentType);
                        multipartFormContent.Add(fileStreamContent, name: "ImageUpload", product.ImageUpload.FileName);
                    }
                    using var response = await _productService.UpdateProductAsync(Id, multipartFormContent);
                    //response.EnsureSuccessStatusCode();
                    if (response.IsSuccessStatusCode) // can use httpResponseMessage.EnsureSuccessStatusCode(); to treat statusCode as error data response
                    {
                        //var responseContent = await response.Content.ReadAsStringAsync();
                        //Console.WriteLine("UploadFileInModel_WithHttpClientAsync response :" + responseContent);
                        TempData["success"] = "Update Product successfully";
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        var errMsg = JsonConvert.DeserializeObject<dynamic>(response.Content.ReadAsStringAsync().Result);
                        TempData["error"] = errMsg;
                        return View(product);
                    }
                }
            }
            else
            {
                TempData["error"] = "Model co 1 vai thu dang bi loi";
                List<string> errors = new List<string>();
                foreach (var value in ModelState.Values)
                {
                    foreach (var error in value.Errors)
                    {
                        errors.Add(error.ErrorMessage);
                    }
                }
                string errorMessage = string.Join("\n", errors);
                //return BadRequest(errorMessage);
                return View(product);
            }
        }

        public async Task<IActionResult> Delete(Guid Id)
        {
            using HttpResponseMessage response = await _productService.DeleteProductAsync(Id);
            var callbackSuccess = (Stream contentStream) =>
            {
                //var responseContent = await response.Content.ReadAsStringAsync();
                //Console.WriteLine("UploadFileInModel_WithHttpClientAsync response :" + responseContent);
                TempData["success"] = "Delete Product successfully";
                return RedirectToAction("Index");
            };
            var callbackFail = (string errMsg) =>
            {
                TempData["error"] = errMsg;
                return RedirectToAction("Index");
            };
            return await HandleResponseResult.HandleResultWithNoResponseData(response, callbackSuccess, callbackFail);
        }

        // Add more quantity to product
        public async Task<IActionResult> AddQuantity(Guid Id)
        {
            var productQuatitiesByProductId = await _productQuantityService.GetProductQuatitiesByProductId(Id);
            ViewBag.Id = Id;
            ViewBag.ProductQuatitiesByProductId = productQuatitiesByProductId;
            return View();
        }

        

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> StoreProductQuantity(ProductQuantityForCreationDto productQuantity)
        {
            if (ModelState.IsValid)
            {
                using HttpResponseMessage response = await _productQuantityService.AddProductQuantityAsync(productQuantity);
                //response.EnsureSuccessStatusCode();
                if (response.IsSuccessStatusCode) // can use httpResponseMessage.EnsureSuccessStatusCode(); to treat statusCode as error data response
                {
                    TempData["success"] = "Add Product Quantity successfully";
                    return RedirectToAction("AddQuantity","Product", new {Id = productQuantity.ProductId});
                }
                else
                {
                    var errMsg = JsonConvert.DeserializeObject<dynamic>(response.Content.ReadAsStringAsync().Result);
                    TempData["error"] = errMsg;
                    return RedirectToAction("AddQuantity", "Product", new { Id = productQuantity.ProductId });
                }
            }
            else
            {
                TempData["error"] = "Model co 1 vai thu dang bi loi";
                List<string> errors = new List<string>();
                foreach (var value in ModelState.Values)
                {
                    foreach (var error in value.Errors)
                    {
                        errors.Add(error.ErrorMessage);
                    }
                }
                string errorMessage = string.Join("\n", errors);
                //return BadRequest(errorMessage);
                return RedirectToAction("AddQuantity", "Product", new { Id = productQuantity.ProductId });
            }
        }
    }
}
