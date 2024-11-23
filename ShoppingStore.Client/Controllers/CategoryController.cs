using Microsoft.AspNetCore.Mvc;
using ShoppingStore.Client.Repository;
using ShoppingStore.Model.Dtos;

namespace ShoppingStore.Client.Controllers
{
    public class CategoryController : Controller
    {
        private readonly CategoryService _categoryService;
        private readonly ProductService _productService;

        public CategoryController(CategoryService categoryService, ProductService productService)
        {
            _categoryService = categoryService;
            _productService = productService;
        }
        public async Task<IActionResult> Index(int? min, int? max,string Slug = "", string sort_by = "product_newest", int pageNumber = 1)
        {
            if (Request.Query["min"].ToString() != "" && min.HasValue == false ||
                Request.Query["max"].ToString() != "" && max.HasValue == false ||
                min == 0 && max == 0 || min < 0 || min > 5000 || max < 0 || max > 5000)
            {
                return RedirectToAction("Index", "Category", new { Slug = Slug });
            }

            using HttpResponseMessage httpResponseMessage = await _productService.GetProductsByCategorySlugAsync(Slug, sort_by,min,max, pageNumber: pageNumber);
            var callbackSuccess = (Stream contentStream, IEnumerable<ProductDto> resultData) =>
            {
                var paginationDataJson = httpResponseMessage.Headers.GetValues("X-Pagination").FirstOrDefault();
                var paginationData = System.Text.Json.JsonSerializer.Deserialize<PaginationMetadata>(paginationDataJson);
                ViewBag.Pager = paginationData;
                ViewBag.CategorySlug = Slug;
                ViewBag.SortBy = sort_by;
                ViewBag.Min = min;
                ViewBag.Max = max;
                return View(resultData);
            };
            var callbackFail = (string errMsg) =>
            {
                TempData["error"] = errMsg;
                ViewBag.Pager = new PaginationMetadata(0, 0, 1);
                ViewBag.SortBy = sort_by;
                ViewBag.Min = min;
                ViewBag.Max = max;
                return View(new List<ProductDto>());
            };
            return await HandleResponseResult.HandleResult<IEnumerable<ProductDto>>(httpResponseMessage, callbackSuccess, callbackFail);
        }

        public IActionResult NextCategoryViewModelMenu(int input, int totalPageCount, string slugQueryString = "")
        {
            if (totalPageCount > 4) totalPageCount = 4;
            if (input > totalPageCount) input = 1;
            if (input < 1) input = totalPageCount;
            HttpContext.Session?.SetInt32("CategoryMenuId", input);
            return ViewComponent("Categories", new
            {
                categoryMenuId = input,
                slugQueryString
            });
        }
    }
}
