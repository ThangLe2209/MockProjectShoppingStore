using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using ShoppingStore.Client.Models.ViewModels;
using ShoppingStore.Client.Repository;
using ShoppingStore.Model.Dtos;
using ShoppingStore.Models;
using System.Net;

namespace ShoppingStore.Client.Controllers
{
    public class CartController : Controller
    {
        private readonly ProductService _productService;
        private readonly ShippingService _shippingService;
        private readonly CouponService _couponService;

        public CartController(ProductService productService, ShippingService shippingService, CouponService couponService)
        {
            _productService = productService;
            _shippingService = shippingService;
            _couponService = couponService;
        }
        public IActionResult Index()
        {
            //dynamic b = TempData["tempDataBeforeLogin"];
            //if (b != null)
            //{
            //    var tempData = JsonConvert.DeserializeObject<dynamic>(b);
            //    string tempTinh = tempData.tinh;
            //    string tempQuan = tempData.quan;
            //    string tempPhuong = tempData.phuong;

            //    ViewBag.TempDataTinh = tempTinh;
            //    ViewBag.TempDataQuan = tempQuan;
            //    ViewBag.TempDataPhuong = tempPhuong;
            //}
            ViewBag.Class = "/Cart";

            //var shippingPriceCookie = Request.Cookies["ShippingPrice"];
            //decimal shippingPrice = 0;

            //if (shippingPriceCookie != null) {
            //	var shippingPriceJson = shippingPriceCookie;
            //	shippingPrice = JsonConvert.DeserializeObject<decimal>(shippingPriceJson);
            //}

            if (Request.Cookies["CouponTitle"] != null) Response.Cookies.Delete("CouponTitle");
            if (Request.Cookies["CouponName"] != null) Response.Cookies.Delete("CouponName");

            List<CartItemModel> cartItems = HttpContext.Session.GetJson<List<CartItemModel>>("Cart") ?? new List<CartItemModel>();
            CartItemViewModel cartVM = new()
            {
                CartItems = cartItems,
                GrandTotal = cartItems.Sum(x => x.Quantity * x.Price),
                ShippingCost = 0,
            };

            int[] arrOptions = Enumerable.Range(1, 100).ToArray();
            ViewBag.ArrOptions = new SelectList(arrOptions);

            return View(cartVM);
        }

        public IActionResult Checkout()
        {
            return View("~/Views/Checkout/Index.cshtml");
        }

        public async Task<IActionResult> Add(Guid Id)
        {
            ProductDto? product = await _productService.GetProductByIdAsync(Id);
            List<CartItemModel> cart = HttpContext.Session.GetJson<List<CartItemModel>>("Cart") ?? new List<CartItemModel>();
            CartItemModel cartItem = cart.Where(c => c.ProductId.CompareTo(Id) == 0).FirstOrDefault();

            if (cartItem == null)
            {
                cart.Add(new CartItemModel(product));
            }
            else
            {
                if (product.Quantity <= cartItem.Quantity)
                {
                    TempData["error"] = $"Maximum products in stock left: {product.Quantity}";
                    return Redirect(Request.Headers["Referer"].ToString());
                }
                cartItem.Quantity += 1;
            }

            HttpContext.Session.SetJson("Cart", cart);

            TempData["success"] = "Add item to cart successfully";
            return Redirect(Request.Headers["Referer"].ToString()); // back to previous page before access this action
        }

        public async Task<IActionResult> Decrease(Guid Id)
        {
            List<CartItemModel> cart = HttpContext.Session.GetJson<List<CartItemModel>>("Cart");
            CartItemModel cartItem = cart.Where(c => c.ProductId == Id).FirstOrDefault();

            if (cartItem.Quantity > 1)
            {
                cartItem.Quantity -= 1;
            }
            else
            {
                cart.RemoveAll(p => p.ProductId.CompareTo(Id) == 0); // Remove that product from cart
            }
            if (cart.Count == 0)
            {
                HttpContext.Session.Remove("Cart");
            }
            else
            {
                HttpContext.Session.SetJson("Cart", cart);
            }

            TempData["success"] = "Decrease item quantity to cart successfully";
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Increase(Guid Id)
        {
            ProductDto? product = await _productService.GetProductByIdAsync(Id);
            List<CartItemModel> cart = HttpContext.Session.GetJson<List<CartItemModel>>("Cart");
            CartItemModel cartItem = cart.Where(c => c.ProductId == Id).FirstOrDefault();

            if (cartItem.Quantity >= 1 && product.Quantity > cartItem.Quantity)
            {
                cartItem.Quantity += 1;

            }
            else
            {
                var errorMsg = product.Quantity == 0 ? $"Product has sold out!" : $"Maximum products in stock left: {product.Quantity}";
                TempData["error"] = errorMsg;
                return RedirectToAction("Index");
            }
            if (cart.Count == 0)
            {
                HttpContext.Session.Remove("Cart");
            }
            else
            {
                HttpContext.Session.SetJson("Cart", cart);
            }

            TempData["success"] = "Increase item quantity to cart successfully";
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Remove(Guid Id)
        {
            List<CartItemModel> cart = HttpContext.Session.GetJson<List<CartItemModel>>("Cart");
            cart.RemoveAll(p => p.ProductId.CompareTo(Id) == 0);
            if (cart.Count == 0)
            {
                HttpContext.Session.Remove("Cart");
            }
            else
            {
                HttpContext.Session.SetJson("Cart", cart);
            }

            TempData["success"] = "Remove item of cart successfully";
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Clear()
        {
            HttpContext.Session.Remove("Cart");

            TempData["success"] = "Clear all item from cart successfully";
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> ChangeSelectOptions(Guid Id, int optionValueChange)
        {
            ProductDto? product = await _productService.GetProductByIdAsync(Id);
            List<CartItemModel> cart = HttpContext.Session.GetJson<List<CartItemModel>>("Cart");
            CartItemModel cartItem = cart.Where(c => c.ProductId == Id).FirstOrDefault();
            if (optionValueChange <= product.Quantity)
            {
                cartItem.Quantity = optionValueChange;

            }
            else
            {
                var errorMsg = product.Quantity == 0 ? $"Product has sold out!" : $"Maximum products in stock left: {product.Quantity}";
                TempData["error"] = errorMsg;
                return StatusCode(400, new { message = errorMsg, redirectToUrl = Url.Action("Index", "Cart") });
            }
            if (cart.Count == 0)
            {
                HttpContext.Session.Remove("Cart");
            }
            else
            {
                HttpContext.Session.SetJson("Cart", cart);
            }

            TempData["success"] = "Update item quantity to cart successfully";
            return Ok(new { success = true, message = "Update item quantity to cart successfully", redirectToUrl = Url.Action("Index", "Cart") });
        }

        [Authorize]
        public IActionResult RedirectPage(string tinh, string quan, string phuong) // not used (just want to fill all geo data after login but failed because api geo is not mine and load unexpected ways not easy to fill data back)
        {
            TempData["tempDataBeforeLogin"] = JsonConvert.SerializeObject(new { tinh, quan, phuong });
            return RedirectToAction("Index", "Cart");
        }

        //[Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken] // https://stackoverflow.com/questions/14473597/include-antiforgerytoken-in-ajax-post-asp-net-mvc
        public async Task<IActionResult> GetShipping(string tinh, string quan, string phuong)
        {
            ShippingDto? existingShipping = null;

            try
            {
                existingShipping = await _shippingService.GetShippingByGeoDataAsync(tinh, quan, phuong);
            }
            catch (HttpRequestException ex)
            {
                // https://stackoverflow.com/questions/65383186/using-httpclient-getfromjsonasync-how-to-handle-httprequestexception-based-on
                // https://stackoverflow.com/questions/33903443/continue-after-httpwebrequest-response-returns-404-error
                if (ex.StatusCode != HttpStatusCode.NotFound)
                {
                    //throw;
                    TempData["error"] = "Some error happend in shipping cost api!";
                    return BadRequest("Some error happend in shipping cost api!");
                }
            }
            decimal shippingPrice = 2; // 2 $ ben ajax thi phai nhan 22000 nua
            if (existingShipping != null)
            {
                shippingPrice = existingShipping.Price;
            }

            // cookies store geo info and price
            var shippingCityJson = JsonConvert.SerializeObject(tinh);
            var shippingDistrictJson = JsonConvert.SerializeObject(quan);
            var shippingWardJson = JsonConvert.SerializeObject(phuong);
            var shippingPriceJson = JsonConvert.SerializeObject(shippingPrice);

            try
            {
                //if (Request.Cookies["ShippingCity"] != null) Response.Cookies.Delete("ShippingCity");
                //if (Request.Cookies["ShippingDistrict"] != null) Response.Cookies.Delete("ShippingDistrict");
                //if (Request.Cookies["ShippingWard"] != null) Response.Cookies.Delete("ShippingWard");
                var cookieOptions = new CookieOptions
                {
                    HttpOnly = true,
                    Expires = DateTime.UtcNow.AddMinutes(30),
                    Secure = true, // using HTTPS
                };

                Response.Cookies.Append("ShippingCity", shippingCityJson, cookieOptions);
                Response.Cookies.Append("ShippingDistrict", shippingDistrictJson, cookieOptions);
                Response.Cookies.Append("ShippingWard", shippingWardJson, cookieOptions);
                Response.Cookies.Append("ShippingPrice", shippingPriceJson, cookieOptions);
            }
            catch (Exception ex)
            {
                var errorMsg = $"Error adding shipping price cookie: {ex.Message}";
                TempData["error"] = errorMsg;
                return BadRequest(errorMsg);
            }
            //return Json(new { shippingPrice });


            // just render shipping field on page by ajax => all select fields will keep in view because page not re-render
            return Ok(new { success = true, message = "Update item quantity to cart successfully", shippingPrice = shippingPrice });
        }

        public IActionResult DeleteShipping()
        {
            if (Request.Cookies["ShippingPrice"] != null) Response.Cookies.Delete("ShippingPrice");
            if (Request.Cookies["ShippingCity"] != null) Response.Cookies.Delete("ShippingCity");
            if (Request.Cookies["ShippingDistrict"] != null) Response.Cookies.Delete("ShippingDistrict");
            if (Request.Cookies["ShippingWard"] != null) Response.Cookies.Delete("ShippingWard");
            return RedirectToAction("Index", "Cart");
        }

        // Ham GetCoupon Code
        //[Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken] // https://stackoverflow.com/questions/14473597/include-antiforgerytoken-in-ajax-post-asp-net-mvc
        public async Task<IActionResult> GetCoupon(string coupon_value, decimal grandTotal)
        {
            try
            {
                var validCoupon = await _couponService.GetCouponValidByNameAsync(coupon_value);
                decimal newGrandTotalData = await _couponService.GetNewGrandTotalByCoupon(grandTotal, validCoupon.Name);

                string couponTitle = validCoupon.Name + " | " + validCoupon?.Description;

                if (couponTitle != null)
                {
                    TimeSpan remainingTime = validCoupon.DateExpired - DateTime.Now;
                    int daysRemaining = remainingTime.Days;

                    if (daysRemaining > 0)
                    {
                        try
                        {
                            var cookieOptions = new CookieOptions
                            {
                                HttpOnly = true,
                                Expires = DateTime.UtcNow.AddMinutes(30),
                                Secure = true, // using HTTPS
                                //SameSite = SameSiteMode.Strict // Kiểm tra tính tương thích trình duyệt - video note (theo mình thì chỗ này nó kiểm tra xem cookie có cross site đc hay ko, hover SameSiteMode trong SameSiteMode.Strict)
                            };

                            Response.Cookies.Append("CouponTitle", couponTitle, cookieOptions);
                            Response.Cookies.Append("CouponName", validCoupon.Name, cookieOptions);

                            decimal shippingPrice = Request.Cookies["ShippingPrice"] == null ? 0 : decimal.Parse(Request.Cookies["ShippingPrice"]);
                            decimal newGrandTotal = newGrandTotalData;
                            decimal newGrandTotalWithShippingPrice = newGrandTotalData + shippingPrice;
                            return Ok(new { success = true, message = "Coupon applied successfully", couponTitle, newGrandTotal, newGrandTotalWithShippingPrice });
                        }
                        catch (Exception ex)
                        {
                            var errorMsg = $"Error adding appy coupon cookie: {ex.Message}";
                            //TempData["error"] = errorMsg;
                            return BadRequest(errorMsg);
                        }
                    }
                    else
                    {
                        return BadRequest("Coupon has expired");
                    }
                }
                //TempData["error"] = $"Coupon Title null";
                return BadRequest("Coupon Title null");
            }
            catch (HttpRequestException ex)
            {
                if (ex.StatusCode == HttpStatusCode.NotFound)
                {
                    //TempData["error"] = $"Coupon Not Valid";
                    return BadRequest("Coupon Not Valid");
                }
                else if (ex.StatusCode == HttpStatusCode.BadRequest)
                {
                    //TempData["error"] = $"Coupon Not Valid";
                    return BadRequest("Coupon Not Valid");
                }
                return StatusCode(500);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message.ToString());
            }
        }

        public IActionResult ThangTestPage()
        {
            return View();
        }
    }
}
