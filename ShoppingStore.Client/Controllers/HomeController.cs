using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.RenderTree;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Newtonsoft.Json;
using ShoppingStore.Client.Models;
using ShoppingStore.Client.Repository;
using ShoppingStore.Model;
using ShoppingStore.Model.Dtos;
using System.ComponentModel;
using System.Diagnostics;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace ShoppingStore.Client.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ProductService _productService;
        private readonly SliderService _sliderService;
        private readonly ContactService _contactService;
        private readonly WishlistService _wishlistService;
        private readonly CompareService _compareService;
        private readonly OrderService _orderService;
        private readonly OrderDetailService _orderDetailService;
        private readonly UserService _userService;
        private readonly CouponService _couponService;

        public HomeController(ILogger<HomeController> logger, IHttpClientFactory httpClientFactory, ProductService productService, SliderService sliderService
            , ContactService contactService, WishlistService wishlistService, CompareService compareService, OrderService orderService
            , OrderDetailService orderDetailService, UserService userService, CouponService couponService)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
            _productService = productService;
            _sliderService = sliderService;
            _contactService = contactService;
            _wishlistService = wishlistService;
            _compareService = compareService;
            _orderService = orderService;
            _orderDetailService = orderDetailService;
            _userService = userService;
            _couponService = couponService;
        }

        public async Task<IActionResult> Index(int? min, int? max, int pageNumber = 1, string sort_by = "product_newest")
        {
            ViewBag.Class = "/";
            await LogIdentityInformation();
            //var c = min.HasValue && !(min is int);
            //var b = min is int;

            if (Request.Query["min"].ToString() != "" && min.HasValue == false ||
                Request.Query["max"].ToString() != "" && max.HasValue == false ||
                min == 0 && max == 0 || min < 0 || min > 5000 || max < 0 || max > 5000)
            {
                return RedirectToAction("Index", "Home");
            }

            // approach1
            //var httpClient = _httpClientFactory.CreateClient("APIClient");
            //var httpResponseMessage = await httpClient.GetAsync("/api/products/");


            //if (httpResponseMessage.IsSuccessStatusCode)
            //{
            //	using var contentStream = await httpResponseMessage.Content.ReadAsStreamAsync();
            //	var options = new JsonSerializerOptions
            //	{
            //		PropertyNameCaseInsensitive = true
            //	};
            //	var products = await JsonSerializer.DeserializeAsync<IEnumerable<ProductDto>>(contentStream, options);
            //	return View(products);
            //}

            // approach2:_httpClient.GetFromJsonAsync => return Json
            //try
            //{
            //    var products = await _productService.GetProductsPaginateAsync("", pageNumber: 1, pageSize: 1);
            //    return View(products);
            //}
            //catch (HttpRequestException ex)
            //{
            //    TempData["error"] = ex.StatusCode.ToString();
            //    return View(new List<ProductDto>());
            //}

            // approach3: _httpClient.GetAsync => return HttpResponseMessage

            // to catch error can treat error status as error message by use httpResponseMessage.EnsureSuccessStatusCode(); or use as below
            // to throw catch error https://learn.microsoft.com/en-us/dotnet/api/system.net.http.httpresponsemessage?view=net-8.0
            //using HttpResponseMessage httpResponseMessage = await _productService.GetProductsPaginateAsync("", pageNumber: pageNumber, pageSize: 1);
            //if (httpResponseMessage.IsSuccessStatusCode) // can use httpResponseMessage.EnsureSuccessStatusCode(); to treat statusCode as error data response => don't need try catch.
            //{
            //    using var contentStream = await httpResponseMessage.Content.ReadAsStreamAsync();
            //    var paginationDataJson = httpResponseMessage.Headers.GetValues("X-Pagination").FirstOrDefault();
            //    var paginationData = System.Text.Json.JsonSerializer.Deserialize<PaginationMetadata>(paginationDataJson);
            //    var options = new JsonSerializerOptions
            //    {
            //        PropertyNameCaseInsensitive = true
            //    };
            //    var products = await System.Text.Json.JsonSerializer.DeserializeAsync<IEnumerable<ProductDto>>(contentStream, options);
            //    //if (products.Count() == 0) return RedirectToAction("Index", "Home");
            //    ViewBag.Pager = paginationData;
            //    return View(products);
            //}
            //else
            //{
            //    var errMsg = JsonConvert.DeserializeObject<dynamic>(httpResponseMessage.Content.ReadAsStringAsync().Result);
            //    TempData["error"] = errMsg;
            //    ViewBag.Pager = new PaginationMetadata(0,0,1);
            //    return View(new List<ProductDto>());
            //}

            // approach4
            var sliders = await _sliderService.GetSlidersAsync();
            ViewBag.Sliders = sliders == null ? new List<SliderDto>() : sliders.Where(s => s.Status == 1).ToList();
            using HttpResponseMessage httpResponseMessage = await _productService.GetProductsPaginateAsync("", sort_by, min, max, pageNumber: pageNumber);
            var callbackSuccess = (Stream contentStream, IEnumerable<ProductDto> resultData) =>
            {
                var paginationDataJson = httpResponseMessage.Headers.GetValues("X-Pagination").FirstOrDefault();
                var paginationData = System.Text.Json.JsonSerializer.Deserialize<PaginationMetadata>(paginationDataJson);
                ViewBag.Pager = paginationData;
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

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error(int statuscode)
        {
            if (statuscode == 404)
            {
                return View("NotFound");
            }
            else
            {
                return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
            }
        }

        public async Task<IActionResult> Contact()
        {
            ViewBag.Class = "/Contact";
            var result = await _contactService.GetContactsAsync();
            return View(result.FirstOrDefault() ?? new ContactDto() { Name = "", Map = "", Email = "", Phone = "" });
        }

        public IActionResult RedirectAuthorizePage(string slug, int pageNumber)
        {
            var redirectPage = Request.Headers.Referer;
            //return RedirectToAction("RedirectPage", "Home", new { slug = slug, pageNumber = pageNumber });
            return RedirectToAction("RedirectPage", "Home", new { referer = redirectPage });
        }

        [Authorize]
        //public IActionResult RedirectPage(string slug, int pageNumber)
        public IActionResult RedirectPage(string referer)
        {
            //return RedirectToAction("Index","Brand", new {slug=slug, pageNumber = pageNumber});
            return Redirect(referer);
        }


        [Authorize]
        public async Task<IActionResult> AddWishlist(Guid Id)
        {
            var userSubId = User.FindFirstValue("sub");
            if (userSubId == null)
            {
                TempData["error"] = "User don't have SubId Claim";
                return StatusCode(400, new { message = "User don't have SubId Claim", redirectToUrl = Url.Action("Index", "Home") });
            }
            var wistList = new WishlistForCreationDto();
            wistList.ProductId = Id;
            wistList.UserId = new Guid(userSubId); //User.Claims?.FirstOrDefault(x => x.Type == "sub")?.Value;
            using HttpResponseMessage response = await _wishlistService.CreateWishlistAsync(wistList);
            if (response.IsSuccessStatusCode) // can use httpResponseMessage.EnsureSuccessStatusCode(); to treat statusCode as error data response
            {
                var createdWishlist = JsonConvert.DeserializeObject<WishlistDto>(response.Content.ReadAsStringAsync().Result);
                var wishListSessionData = HttpContext.Session.GetJson<List<WishlistWithProductAndUserDto>>("WishlistData");
                wishListSessionData.Add(new WishlistWithProductAndUserDto() { Wishlist = createdWishlist });
                HttpContext.Session.SetJson("WishlistData", wishListSessionData);
                TempData["success"] = "Add to Wishlist successfully";
                return Ok(new { success = true, message = "Add to Wishlist successfully", redirectToUrl = Url.Action("Index", "Home") });
            }
            else
            {
                var errMsg = JsonConvert.DeserializeObject<dynamic>(response.Content.ReadAsStringAsync().Result);
                TempData["error"] = errMsg;
                return StatusCode(400, new { message = errMsg, redirectToUrl = Url.Action("Index", "Home") });
            }
        }

        [Authorize]
        public async Task<IActionResult> Wishlist()
        {
            ViewBag.Class = "/Wishlist";
            var userSubId = User.FindFirstValue("sub");
            var result = await _wishlistService.GetWishlistWithProductAndUser(new Guid(userSubId));
            return View(result);
        }

        [Authorize]
        public async Task<IActionResult> DeleteWishlist(Guid Id)
        {
            ViewBag.Class = "/Wishlist";
            using HttpResponseMessage response = await _wishlistService.DeleteWishlistAsync(Id);
            if (response.IsSuccessStatusCode) // can use httpResponseMessage.EnsureSuccessStatusCode(); to treat statusCode as error data response
            {
                var wishListSessionData = HttpContext.Session.GetJson<List<WishlistWithProductAndUserDto>>("WishlistData");
                wishListSessionData.Remove(wishListSessionData.Single(w => w.Wishlist.Id == Id));
                HttpContext.Session.SetJson("WishlistData", wishListSessionData);
                TempData["success"] = "Remove Wishlist successfully";
                return RedirectToAction("Wishlist", "Home");
            }
            else
            {
                var errMsg = JsonConvert.DeserializeObject<dynamic>(response.Content.ReadAsStringAsync().Result);
                TempData["error"] = errMsg;
                return RedirectToAction("Wishlist", "Home");
            }
        }

        [Authorize]
        public async Task<IActionResult> AddCompare(Guid Id)
        {
            var userSubId = User.FindFirstValue("sub");
            if (userSubId == null)
            {
                TempData["error"] = "User don't have SubId Claim";
                return StatusCode(400, new { message = "User don't have SubId Claim", redirectToUrl = Url.Action("Index", "Home") });
            }
            var compare = new CompareForCreationDto();
            compare.ProductId = Id;
            compare.UserId = new Guid(userSubId); //User.Claims?.FirstOrDefault(x => x.Type == "sub")?.Value;
            using HttpResponseMessage response = await _compareService.CreateCompareAsync(compare);
            if (response.IsSuccessStatusCode) // can use httpResponseMessage.EnsureSuccessStatusCode(); to treat statusCode as error data response
            {
                var createdCompare = JsonConvert.DeserializeObject<CompareDto>(response.Content.ReadAsStringAsync().Result);
                var compareSessionData = HttpContext.Session.GetJson<List<CompareWithProductAndUserDto>>("CompareData");
                compareSessionData.Add(new CompareWithProductAndUserDto() { Compare = createdCompare });
                HttpContext.Session.SetJson("CompareData", compareSessionData);
                TempData["success"] = "Add to Compare successfully";
                return Ok(new { success = true, message = "Add to Compare successfully", redirectToUrl = Url.Action("Index", "Home") });
            }
            else
            {
                var errMsg = JsonConvert.DeserializeObject<dynamic>(response.Content.ReadAsStringAsync().Result);
                TempData["error"] = errMsg;
                return StatusCode(400, new { message = errMsg, redirectToUrl = Url.Action("Index", "Home") });
            }
        }

        [Authorize]
        public async Task<IActionResult> Compare()
        {
            ViewBag.Class = "/Compare";
            var userSubId = User.FindFirstValue("sub");
            var result = await _compareService.GetCompareWithProductAndUser(new Guid(userSubId));
            return View(result);
        }

        [Authorize]
        public async Task<IActionResult> DeleteCompare(Guid Id)
        {
            ViewBag.Class = "/Compare";
            using HttpResponseMessage response = await _compareService.DeleteCompareAsync(Id);
            if (response.IsSuccessStatusCode) // can use httpResponseMessage.EnsureSuccessStatusCode(); to treat statusCode as error data response
            {
                var compareSessionData = HttpContext.Session.GetJson<List<CompareWithProductAndUserDto>>("CompareData");
                compareSessionData.Remove(compareSessionData.Single(c => c.Compare.Id == Id));
                HttpContext.Session.SetJson("CompareData", compareSessionData);
                TempData["success"] = "Remove Compare successfully";
                return RedirectToAction("Compare", "Home");
            }
            else
            {
                var errMsg = JsonConvert.DeserializeObject<dynamic>(response.Content.ReadAsStringAsync().Result);
                TempData["error"] = errMsg;
                return RedirectToAction("Compare", "Home");
            }
        }

        [Authorize]
        public async Task<IActionResult> OrdersHistory()
        {
            ViewBag.Class = "/OrdersHistory";

            if (Request.Cookies["successfullCheckOut"] != null)
            {
                //ViewBag.SuccessfullCheckOut = JsonConvert.DeserializeObject<OrderDto>(Request.Cookies["successfullCheckOut"]); // cant use because C# auto convert to Model to hidden
                var result = JsonConvert.DeserializeObject<dynamic>(Request.Cookies["successfullCheckOut"]);
                ViewBag.SuccessfullCheckOut = result;
                Response.Cookies.Delete("successfullCheckOut");
            }
            else ViewBag.SuccessfullCheckOut = "";

            var userSubId = User.FindFirstValue("sub");
            var userEmail = User.FindFirstValue("email");
            if (userEmail == null)
            {
                TempData["error"] = "User don't have email";
                return RedirectToAction("Index", "Home");
            }
            var ordersByUserEmail = await _orderService.GetOrdersByUserEmailAsync(userEmail);
            ViewBag.UserEmail = userEmail;
            return View(ordersByUserEmail);
        }

        [Authorize]
        public async Task<IActionResult> ViewOrder(string ordercode)
        {
            try
            {
                ViewBag.Class = "/OrdersHistory";
                var order = await _orderService.GetOrderByOrderCodeAsync(ordercode);

                ViewBag.DiscountPercent = 0;
                ViewBag.DiscountDecrease = 0;
                ViewBag.CouponCode = order?.CouponCode;
                if (order?.CouponCode != null)
                {
                    var coupon = await _couponService.GetCouponExistedByNameAsync(order.CouponCode);
                    ViewBag.DiscountPercent = coupon?.DiscountPercent;
                    ViewBag.DiscountDecrease = coupon?.DiscountDecrease;
                }

                ViewBag.orderStatus = order?.Status;
                // Get Shipping Cost
                ViewBag.ShippingCost = order?.ShippingCost;
                var orderDetails = await _orderDetailService.GetOrderDetailsByOrderCodeAsync(ordercode);

                return View(orderDetails);
            }
            catch (Exception ex)
            {
                TempData["error"] = "Some error occures when accessing View Order Page";
                return RedirectToAction("OrdersHistory", "Home");
            }
        }

        [Authorize]
        public async Task<IActionResult> CancelOrder(string ordercode) // do lazy way should do Patch as Checkout Controller update Order Quantity
        {
            using HttpResponseMessage response = await _orderService.UpdateOrderAsync(ordercode, 3);
            //response.EnsureSuccessStatusCode();
            if (response.IsSuccessStatusCode) // can use httpResponseMessage.EnsureSuccessStatusCode(); to treat statusCode as error data response
            {
                //var responseContent = await response.Content.ReadAsStringAsync();
                //Console.WriteLine("UploadFileInModel_WithHttpClientAsync response :" + responseContent);
                TempData["success"] = "Cancel Order successfully";
                return RedirectToAction("OrdersHistory", "Home");
            }
            else
            {
                var errMsg = JsonConvert.DeserializeObject<dynamic>(response.Content.ReadAsStringAsync().Result);
                TempData["error"] = errMsg;
                return RedirectToAction("OrdersHistory", "Home");
            }
        }


        [Authorize]
        public async Task<IActionResult> Account()
        {
            var userLogin = User.FindFirstValue("amr");
            if (userLogin != "pwd")
            {
                return RedirectToAction("Index", "Home");
            }
            ViewBag.Class = "/Account";
            var userEmail = User.FindFirstValue("email");
            var user = await _userService.GetUserByEmailAsync(userEmail);

            ViewBag.Countries = new SelectList(
                new[] {
                    new { Id = "be", Value = "Belgium" },
                    new { Id = "us", Value = "United States of America" },
                    new { Id = "in", Value = "India" },
                    new { Id = "vi", Value = "VietNam" } },
                "Id",
                "Value", user.Claims.FirstOrDefault(c => c.Type == "country").Value);

            var jsonParent = JsonConvert.SerializeObject(user);
            UserForEditDto userEdit = JsonConvert.DeserializeObject<UserForEditDto>(jsonParent);
            ViewBag.UserId = user.Id;
            //ViewBag.UserRoleId = user.UserRoleId;
            return View(userEdit);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Account(Guid userId, UserForEditDto updatedUser)
        {
            ViewBag.Class = "/Account";
            ViewBag.Countries = new SelectList(
                new[] {
                    new { Id = "be", Value = "Belgium" },
                    new { Id = "us", Value = "United States of America" },
                    new { Id = "in", Value = "India" },
                    new { Id = "vi", Value = "VietNam" } },
                "Id",
                "Value", updatedUser.Country);

            if (userId == Guid.Empty)
            {
                TempData["error"] = "Null Guid UserId";
                return View(updatedUser);
            }

            if (ModelState.IsValid)
            {
                //add data to db
                //TempData["success"] = "Model da ok het";
                using HttpResponseMessage response = await _userService.UpdateUserAsync(userId, updatedUser);
                //response.EnsureSuccessStatusCode();
                if (response.IsSuccessStatusCode) // can use httpResponseMessage.EnsureSuccessStatusCode(); to treat statusCode as error data response
                {
                    //var responseContent = await response.Content.ReadAsStringAsync();
                    //Console.WriteLine("UploadFileInModel_WithHttpClientAsync response :" + responseContent);
                    TempData["success"] = "Update User successfully. Please login again to refesh your information!";
                    return RedirectToAction("Logout", "Authentication");
                }
                else
                {
                    var errMsg = JsonConvert.DeserializeObject<dynamic>(response.Content.ReadAsStringAsync().Result);
                    TempData["error"] = errMsg;
                    ViewBag.UserId = userId;
                    //ViewBag.UserRoleId = updatedUser.UserRoleId;
                    return View(updatedUser);
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
                ViewBag.UserId = userId;
                //ViewBag.UserRoleId = updatedUser.UserRoleId;
                return View(updatedUser);
            }
        }



        public async Task LogIdentityInformation()
        {
            // get the saved identity token
            var identityToken = await HttpContext
                .GetTokenAsync(OpenIdConnectParameterNames.IdToken);

            // get the saved access token
            var accessToken = await HttpContext
                .GetTokenAsync(OpenIdConnectParameterNames.AccessToken);

            // get the refresh token
            var refreshToken = await HttpContext
                .GetTokenAsync(OpenIdConnectParameterNames.RefreshToken);

            var userClaimsStringBuilder = new StringBuilder();
            foreach (var claim in User.Claims)
            {
                userClaimsStringBuilder.AppendLine(
                    $"Claim type: {claim.Type} - Claim value: {claim.Value}");
            }

            // log token & claims
            _logger.LogInformation($"Identity token & user claims: " +
                $"\n{identityToken} \n{userClaimsStringBuilder}");
            _logger.LogInformation($"Access token: " +
                $"\n{accessToken}");
            _logger.LogInformation($"Refresh token: " +
                $"\n{refreshToken}");
        }
    }
}
