using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ShoppingStore.Client.Models.ViewModels;
using ShoppingStore.Client.Repository;
using ShoppingStore.Model.Dtos;
using System.Collections.Generic;
using System.Security.Claims;

namespace ShoppingStore.Client.Controllers
{
    public class ProductController : Controller
    {
        private readonly ProductService _productService;
        private readonly RatingService _ratingService;

        public ProductController(ProductService productService, RatingService ratingService)
        {
            _productService = productService;
            _ratingService = ratingService;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Search1(string searchTerm, int pageNumber = 1)
        {
            return RedirectToAction("Search", new {searchTerm = searchTerm, pageNumber = pageNumber});
        }

        public async Task<IActionResult> Search(int? min, int?max, string searchTerm = "",string sort_by = "product_newest", int pageNumber = 1)
        {
            if (Request.Query["min"].ToString() != "" && min.HasValue == false ||
                Request.Query["max"].ToString() != "" && max.HasValue == false ||
                min == 0 && max == 0 || min < 0 || min > 5000 || max < 0 || max > 5000)
            {
                return RedirectToAction("Search", "Product", new { searchTerm = searchTerm });
            }

            using HttpResponseMessage httpResponseMessage = await _productService.GetProductsPaginateAsync(searchTerm, sort_by,min,max, pageNumber: pageNumber);
            var callbackSuccess = (Stream contentStream, IEnumerable<ProductDto> resultData) =>
            {
                var paginationDataJson = httpResponseMessage.Headers.GetValues("X-Pagination").FirstOrDefault();
                var paginationData = System.Text.Json.JsonSerializer.Deserialize<PaginationMetadata>(paginationDataJson);
                ViewBag.Pager = paginationData;
                ViewBag.Keyword = searchTerm;
                ViewBag.SortBy = sort_by;
                ViewBag.Min = min;
                ViewBag.Max = max;
                return View(resultData);
            };
            var callbackFail = (string errMsg) =>
            {
                TempData["error"] = errMsg;
                ViewBag.Pager = new PaginationMetadata(0, 0, 1);
                ViewBag.Keyword = searchTerm;
                ViewBag.SortBy = sort_by;
                ViewBag.Min = min;
                ViewBag.Max = max;
                return View(new List<ProductDto>());
            };
            return await HandleResponseResult.HandleResult<IEnumerable<ProductDto>>(httpResponseMessage, callbackSuccess, callbackFail);
        }

        public async Task<IActionResult> Details(Guid Id)
        {
            if (Id == null) return RedirectToAction("Index","Home");
            var productsById = await _productService.GetProductByIdAsync(Id);

            using HttpResponseMessage httpResponseMessage = await _productService.GetProductsByCategorySlugAsync(slug: productsById.Category.Slug,min:null,max:null, sortBy: "product_newest",pageNumber: 1, pageSize: 8);
            var callbackSuccess = (Stream contentStream, IEnumerable<ProductDto> resultData) =>
            {
                //related Product
                ViewBag.RelatedProducts = resultData
                    .Where(p => p.Id != productsById.Id)
                    .Take(3).ToList();
                
                var totalAnotherPage = (int)Math.Floor((double)resultData.Count() / 3);
				Dictionary<int, IEnumerable<ProductDto>> relatedProductsAnotherPage = new Dictionary<int, IEnumerable<ProductDto>>();
                for (int i = 1; i < totalAnotherPage; i++)
                {
				    relatedProductsAnotherPage.Add(i, Utilities.GetRelatedProduct(i, resultData, productsById));
                }
				ViewBag.relatedProductsAnotherPage = relatedProductsAnotherPage;

				var viewModel = new ProductDetailsViewModel
                {
                    ProductDetails = productsById,
                    //RatingDetails = new RatingModel()
                };
                return View(viewModel);
            };
            var callbackFail = (string errMsg) =>
            {
                TempData["error"] = errMsg;
                return RedirectToAction("Index","Home");
                //return View(new ProductDto());
            };
            return await HandleResponseResult.HandleResult<IEnumerable<ProductDto>>(httpResponseMessage, callbackSuccess, callbackFail);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CommentProduct(RatingForCreationDto rating)
        {
            if (ModelState.IsValid)
            {
                rating.Name = User.Identity?.Name;
                rating.Email = User.Claims?.FirstOrDefault(x => x.Type == "email")?.Value;
                using HttpResponseMessage httpResponseMessage = await _ratingService.CreateRatingAsync(rating);
                if (httpResponseMessage.IsSuccessStatusCode) // can use httpResponseMessage.EnsureSuccessStatusCode(); to treat statusCode as error data response
                {
                    TempData["success"] = "Them danh gia thanh cong!";
                    return Redirect(Request.Headers["Referer"]);
                }
                else
                {
                    var errMsg = JsonConvert.DeserializeObject<dynamic>(httpResponseMessage.Content.ReadAsStringAsync().Result);
                    TempData["error"] = errMsg;
                    return Redirect(Request.Headers["Referer"]);
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
                return RedirectToAction("Detail","Product", new { id = rating.ProductId });
            }
        }

        public IActionResult NextRatingViewModelPage(string productId, int input, int totalPageCount)
        {
            if (totalPageCount > 4) totalPageCount = 4;
            if (input > totalPageCount) input = 1;
            if(input < 1) input = totalPageCount;
            return ViewComponent("Ratings", new
            {
                productId = new Guid(productId),
                ratingPageId = input
            });
        }
    }
}
