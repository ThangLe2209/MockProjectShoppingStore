using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ShoppingStore.Client.Repository;
using System.Collections.Generic;

namespace ShoppingStore.Client.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class DashboardController : Controller
    {
        private readonly ProductService _productService;
        private readonly OrderService _orderService;
        private readonly CategoryService _categoryService;
        private readonly BrandService _brandService;
        private readonly UserService _userService;
        private readonly StatisticService _statisticService;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public DashboardController(ProductService productService, OrderService orderService, CategoryService categoryService, BrandService brandService
            , UserService userService, StatisticService statisticService, IWebHostEnvironment webHostEnvironment)
        {
            _productService = productService;
            _orderService = orderService;
            _categoryService = categoryService;
            _brandService = brandService;
            _userService = userService;
            _statisticService = statisticService;
            _webHostEnvironment = webHostEnvironment;
        }
        public async Task<IActionResult> Index()
        {
            ViewBag.Class = "/Admin/Dashboard";

            var count_product = (await _productService.GetProductsAsync(""))?.Count() ?? 0;
            var count_order = (await _orderService.GetOrdersAsync())?.Count() ?? 0;
            var count_category = (await _categoryService.GetCategoriesAsync())?.Count() ?? 0;
            var count_user = (await _userService.GetUsersAsync())?.Count() ?? 0;

            ViewBag.CountProduct = count_product;
            ViewBag.CountOrder = count_order;
            ViewBag.CountCategory = count_category;
            ViewBag.CountUser = count_user;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> GetChartData()
        {
            ViewBag.Class = "/Admin/Dashboard";
            using HttpResponseMessage response = await _statisticService.GetStatisticsAsync();
            //response.EnsureSuccessStatusCode();
            if (response.IsSuccessStatusCode) // can use httpResponseMessage.EnsureSuccessStatusCode(); to treat statusCode as error data response
            {
                //var responseContent = await response.Content.ReadAsStringAsync();
                //Console.WriteLine("UploadFileInModel_WithHttpClientAsync response :" + responseContent);
                var dataJson = JsonConvert.DeserializeObject<List<dynamic>>(response.Content.ReadAsStringAsync().Result);
                var dataObject = JsonConvert.DeserializeObject<List<StaticTemp>>(response.Content.ReadAsStringAsync().Result);
                //TempData["success"] = "Get Statistic successfully";
                var data = Json(dataObject);
                var b = Json(dataJson);
                var c = dataJson;
                return data;
                //return Ok(new { data = data[0] });
            }
            else
            {
                var errMsg = JsonConvert.DeserializeObject<dynamic>(response.Content.ReadAsStringAsync().Result);
                //TempData["error"] = errMsg;
                return BadRequest(errMsg);
            }
        }

        [HttpPost]
        public async Task<IActionResult> GetChartDataBySelect(DateTime startDate, DateTime endDate)
        {
            ViewBag.Class = "/Admin/Dashboard";
            using HttpResponseMessage response = await _statisticService.GetStatisticsByStartEndDateAsync(startDate, endDate);
            //response.EnsureSuccessStatusCode();
            if (response.IsSuccessStatusCode) // can use httpResponseMessage.EnsureSuccessStatusCode(); to treat statusCode as error data response
            {
                //var responseContent = await response.Content.ReadAsStringAsync();
                //Console.WriteLine("UploadFileInModel_WithHttpClientAsync response :" + responseContent);
                var dataJson = JsonConvert.DeserializeObject<List<dynamic>>(response.Content.ReadAsStringAsync().Result);
                var dataObject = JsonConvert.DeserializeObject<List<StaticTemp>>(response.Content.ReadAsStringAsync().Result);
                //TempData["success"] = "Get Statistic successfully";
                var data = Json(dataObject);
                var b = Json(dataJson);
                var c = dataJson;
                return data;
                //return Ok(new { data = data[0] });
            }
            else
            {
                var errMsg = JsonConvert.DeserializeObject<dynamic>(response.Content.ReadAsStringAsync().Result);
                //TempData["error"] = errMsg;
                return BadRequest(errMsg);
            }
        }
    }
}

public class StaticTemp
{
    public DateTime date { get; set; }
    public string sold { get; set; }
    public int quantity { get; set; }
    public int revenue { get; set; }
    public int profit { get; set; }
}
