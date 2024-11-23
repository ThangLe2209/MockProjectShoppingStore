using Microsoft.AspNetCore.Mvc;
using ShoppingStore.Client.Repository;
using ShoppingStore.Model.Dtos;
using ShoppingStore.Models;
using System.Net.Mail;
using System.Runtime.CompilerServices;

namespace ShoppingStore.Client.Controllers
{
    public class BrandController : Controller
    {
        private readonly BrandService _brandService;
        private readonly ProductService _productService;

        public BrandController(BrandService brandService, ProductService productService)
        {
            _brandService = brandService;
            _productService = productService;
        }
        public async Task<IActionResult> Index(int? min, int? max, string Slug = "", string sort_by = "product_newest", int pageNumber = 1)
        {
            if (Request.Query["min"].ToString() != "" && min.HasValue == false ||
                Request.Query["max"].ToString() != "" && max.HasValue == false ||
                min == 0 && max == 0 || min < 0 || min > 5000 || max < 0 || max > 5000)
            {
                return RedirectToAction("Index", "Brand", new { Slug = Slug });
            }

            using HttpResponseMessage httpResponseMessage = await _productService.GetProductsByBrandSlug(Slug, sort_by, min,max, pageNumber: pageNumber);
            var callbackSuccess = (Stream contentStream, IEnumerable<ProductDto> resultData) =>
            {
                var paginationDataJson = httpResponseMessage.Headers.GetValues("X-Pagination").FirstOrDefault();
                var paginationData = System.Text.Json.JsonSerializer.Deserialize<PaginationMetadata>(paginationDataJson);
                ViewBag.Pager = paginationData;
                ViewBag.BrandSlug = Slug;
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

        public IActionResult NextBrandViewModelMenu(int input, int totalPageCount, string slugQueryString = "")
        {
            if (totalPageCount > 4) totalPageCount = 4;
            if (input > totalPageCount) input = 1;
            if (input < 1) input = totalPageCount;
            //int brandMenuId = HttpContext.Session.GetInt32("BrandMenuId") ?? 1;
            HttpContext.Session.SetInt32("BrandMenuId", input);
            return ViewComponent("Brands", new
            {
                brandMenuId = input,
                slugQueryString
            });
        }
    }
}
