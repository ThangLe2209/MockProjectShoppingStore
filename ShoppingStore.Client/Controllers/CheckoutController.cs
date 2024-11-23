using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ShoppingStore.Client.Repository;
using ShoppingStore.Client.Services.Momo;
using ShoppingStore.Client.Services.Vnpay;
using ShoppingStore.Model;
using ShoppingStore.Model.Dtos;
using ShoppingStore.Models;
using System.Net.Http;
using System.Security.Claims;
using System.Text;

namespace ShoppingStore.Controllers
{
    [Authorize]
    public class CheckoutController : Controller
    {
        private readonly IMomoService _momoService;
        private readonly IVnPayService _vnPayService;
        private readonly ExternalPaymentService _externalPaymentService;
        private readonly OrderService _orderService;
        private readonly OrderDetailService _orderDetailService;
        private readonly ProductService _productService;
        private readonly CouponService _couponService;
        private readonly StatisticProductOrderService _statisticProductOrderService;
        //private readonly IEmailSender _emailSender;

        public CheckoutController(OrderService orderService, OrderDetailService orderDetailService, ProductService productService, CouponService couponService
            , StatisticProductOrderService statisticProductOrderService, IMomoService momoService, IVnPayService vnPayService, ExternalPaymentService externalPaymentService)
        {
            _orderService = orderService ?? throw new ArgumentNullException(nameof(orderService));
            _orderDetailService = orderDetailService ?? throw new ArgumentNullException(nameof(orderDetailService));
            _productService = productService ?? throw new ArgumentNullException(nameof(productService));
            _couponService = couponService ?? throw new ArgumentNullException(nameof(couponService));
            _statisticProductOrderService = statisticProductOrderService ?? throw new ArgumentNullException(nameof(statisticProductOrderService));
            _momoService = momoService ?? throw new ArgumentNullException(nameof(momoService));
            _vnPayService = vnPayService ?? throw new ArgumentNullException(nameof(vnPayService));
            _externalPaymentService = externalPaymentService ?? throw new ArgumentNullException(nameof(externalPaymentService));
            //_emailSender = emailSender ?? throw new ArgumentNullException(nameof(emailSender));
        }

        public IActionResult Index()
        {
            return RedirectToAction("Index", "Cart");
        }

        public async Task<IActionResult> Checkout(string PaymentMethod, string PaymentId) // PaymentMethod, PaymentId chi dc them khi thanh toan external
        {
            Dictionary<string, ErrorModel> errorMessageLists = new();
            try
            {
                if (Request.Cookies["ShippingCity"] == null || Request.Cookies["ShippingDistrict"] == null
                    || Request.Cookies["ShippingWard"] == null)
                {
                    TempData["error"] = "Please calculate shipping cost first!";
                    return RedirectToAction("Index", "Cart");
                }

                var couponCode = Request.Cookies["CouponName"];

                CouponDto? couponValid = null;
                if (couponCode != null)
                {
                    using var getCouponResponse = await _couponService.GetCouponValidByNameWithErrorMessageAsync(couponCode);
                    if (getCouponResponse.IsSuccessStatusCode)
                    {
                        couponValid = JsonConvert.DeserializeObject<CouponDto>(getCouponResponse.Content.ReadAsStringAsync().Result);
                    }
                    else
                    {
                        errorMessageLists.Add("getCouponResponse", new ErrorModel
                        {
                            isSuccess = getCouponResponse.IsSuccessStatusCode
                        ,
                            dataResponse = JsonConvert.DeserializeObject<dynamic>(getCouponResponse.Content.ReadAsStringAsync().Result)
                        });
                    }
                    getCouponResponse.EnsureSuccessStatusCode();
                }


                var userEmail = User.FindFirstValue("email");
                if (userEmail == null)
                {
                    TempData["error"] = "User don't have email claim!";
                    return RedirectToAction("Index", "Cart");
                }
                else
                {
                    var orderCode = Guid.NewGuid().ToString();

                    var orderDetailsCreate = new List<OrderDetailForCreationDto>();
                    var statisticalProductOrdersCreate = new List<StatisticalProductOrderForCreationDto>();
                    List<CartItemModel> cartItems = HttpContext.Session.GetJson<List<CartItemModel>>("Cart") ?? new List<CartItemModel>();
                    Dictionary<Guid, string> updatedPartialProductList = new();

                    foreach (var cartItem in cartItems)
                    {
                        var product = await _productService.GetProductByIdAsync(cartItem.ProductId);
                        if (product.Quantity - cartItem.Quantity < 0)
                        {
                            //await _orderService.DeleteOrderAsync(createdOrder.Id);
                            var errMsg = product.Quantity == 0 ? $"Product {product.Name} has sold out!" : $"Product {product.Name} only have {product.Quantity} in stock left!";
                            TempData["error"] = errMsg;
                            return RedirectToAction("Index", "Cart");
                        }

                        var statisticalProductOrders = new StatisticalProductOrderForCreationDto()
                        {
                            UserEmail = userEmail,
                            OrderCode = orderCode,
                            ProductId = cartItem.ProductId,
                            ProductPrice = cartItem.Price,
                            Quantity = cartItem.Quantity,
                            TotalRevenue = cartItem.Total,
                            CouponCode = couponCode
                        };
                        statisticalProductOrdersCreate.Add(statisticalProductOrders);

                        var orderDetails = new OrderDetailForCreationDto()
                        {
                            UserName = userEmail,
                            OrderCode = orderCode,
                            ProductId = cartItem.ProductId,
                            Price = cartItem.Price,
                            Quantity = cartItem.Quantity,
                        };
                        orderDetailsCreate.Add(orderDetails);

                        var newProductQuantity = product.Quantity - cartItem.Quantity;
                        var newProductSold = product.Sold + cartItem.Quantity;
                        var updatedProductJsonPatch = new JsonPatchDocument<ProductForEditWithQuantityDto>();
                        updatedProductJsonPatch.Replace(p => p.Quantity, newProductQuantity);
                        updatedProductJsonPatch.Replace(p => p.Sold, newProductSold);

                        var serializedItemToUpdate = JsonConvert.SerializeObject(updatedProductJsonPatch);
                        updatedPartialProductList.Add(cartItem.ProductId, serializedItemToUpdate);
                        //StringContent requestContent = new StringContent(serializedItemToUpdate, Encoding.UTF8, "application/json");
                        //using var updatedProduct = await _productService.PartiallyUpdateProductAsync(cartItem.ProductId, requestContent);
                        //try
                        //{
                        //	updatedProduct.EnsureSuccessStatusCode();
                        //}
                        //catch (Exception ex) 
                        //{
                        //                   var errMsg = JsonConvert.DeserializeObject<dynamic>(updatedProduct.Content.ReadAsStringAsync().Result);
                        //                   TempData["error"] = errMsg;
                        //                   return RedirectToAction("Index", "Cart");
                        //               }
                    }

                    // Update Product Quantity By Order Details (simulator rollback trasaction feature)
                    using var updatedProductQuantityAndSold = await _productService.PartiallyUpdateProductsAsync(updatedPartialProductList);
                    try
                    {
                        updatedProductQuantityAndSold.EnsureSuccessStatusCode();
                    }
                    catch (Exception ex)
                    {
                        var errMsg = JsonConvert.DeserializeObject<dynamic>(updatedProductQuantityAndSold.Content.ReadAsStringAsync().Result);
                        TempData["error"] = errMsg;
                        return RedirectToAction("Index", "Cart");
                    }

                    //==== Order Create
                    var orderItem = new OrderForCreationDto()
                    {
                        OrderCode = orderCode,
                        UserName = userEmail,
                        Status = 1,
                        //CreatedDate = DateTime.Now, // replace use SaveChangesAsync auto update 
                    };

                    // Momo
                    if (PaymentMethod == "VnPay")
                    {
                        orderItem.PaymentMethod = "VnPay " + PaymentId;
                    }
                    else if (PaymentMethod == "Momo")
                    {
                        orderItem.PaymentMethod = "Momo " + PaymentId;
                    }
                    else orderItem.PaymentMethod = "COD";

                    //===== order Shipping Price Code
                    var shippingPriceCookie = Request.Cookies["ShippingPrice"];
                    decimal shippingPrice = 0;

                    if (shippingPriceCookie != null)
                    {
                        var shippingPriceJson = shippingPriceCookie;
                        shippingPrice = JsonConvert.DeserializeObject<decimal>(shippingPriceJson);
                    }
                    orderItem.ShippingCost = shippingPrice;


                    //===== order Coupon Code
                    if (couponCode != null)
                    {
                        orderItem.CouponCode = couponCode;

                        // decrease coupon quantity
                        var updatedCouponJsonPatch = new JsonPatchDocument<CouponForEditDto>();
                        var newCouponQuantity = couponValid.Quantity - 1;
                        updatedCouponJsonPatch.Replace(c => c.Quantity, newCouponQuantity);
                        var serializedCouponToUpdate = JsonConvert.SerializeObject(updatedCouponJsonPatch);
                        StringContent requestCouponContent = new StringContent(serializedCouponToUpdate, Encoding.UTF8, "application/json");
                        using var updatedCoupon = await _couponService.PartiallyUpdateCouponAsync(couponValid.Id, requestCouponContent);
                        updatedCoupon.EnsureSuccessStatusCode();
                    }

                    using var createOrderResponse = await _orderService.CreateOrderAsync(orderItem);
                    createOrderResponse.EnsureSuccessStatusCode();
                    var createdOrder = JsonConvert.DeserializeObject<OrderDto>(createOrderResponse.Content.ReadAsStringAsync().Result); // if need created Data


                    // Order Details Create
                    using var createOrderDetailsResponse = await _orderDetailService.CreateOrderDetailsAsync(orderDetailsCreate);
                    createOrderDetailsResponse.EnsureSuccessStatusCode();

                    // StatisticOrderProduct Create
                    using var createStatisticOrderProductResponse = await _statisticProductOrderService.CreateStatisticalProductOrdersAsync(statisticalProductOrdersCreate);
                    createStatisticOrderProductResponse.EnsureSuccessStatusCode();
                    HttpContext.Session.Remove("Cart");

                    // delete cookie
                    if (Request.Cookies["ShippingPrice"] != null) Response.Cookies.Delete("ShippingPrice");
                    if (Request.Cookies["ShippingCity"] != null) Response.Cookies.Delete("ShippingCity");
                    if (Request.Cookies["ShippingDistrict"] != null) Response.Cookies.Delete("ShippingDistrict");
                    if (Request.Cookies["ShippingWard"] != null) Response.Cookies.Delete("ShippingWard");

                    //Send mail order success
                    var receiver = "demologin979@gmail.com";
                    var subject = "Dang nhap tren thiet bi thanh cong!";
                    var message = "Dang nhap thanh cong, trai nghiem dich vu nhe!";
                    //await _emailSender.SendEmailAsync(receiver, subject, message);
                    TempData["success"] = "Checkout successfully! Please wait for reviewing process";

                    //return RedirectToAction("Index", "Cart");
                    var cookieOptions = new CookieOptions
                    {
                        HttpOnly = true,
                        Expires = DateTime.UtcNow.AddMinutes(5),
                        Secure = true, // using HTTPS
                    };
                    Response.Cookies.Append("successfullCheckOut", JsonConvert.SerializeObject(createdOrder) , cookieOptions);
                    return RedirectToAction("OrdersHistory", "Home");
                    //return RedirectToAction("OrdersHistory", "Home", new {successfullCheckOut = true});
                }
            }
            catch (Exception ex)
            {
                bool isErrorFromApi = false;
                var errorEntity = errorMessageLists.FirstOrDefault(e => e.Value.isSuccess == false && (isErrorFromApi= true));
                if (isErrorFromApi)
                {
                    var responseError = errorEntity.Value.dataResponse;
                    TempData["error"] = responseError;
                }
                else TempData["error"] = "Some error happen with Checkout Action!";
                return RedirectToAction("Index", "Cart");
            }
        }


        [HttpGet]
        public async Task<IActionResult> PaymentCallBack()
        {
            var response = _momoService.PaymentExecuteAsync(HttpContext.Request.Query);
            var requestQuery = HttpContext.Request.Query;

            //if (requestQuery["resultCode"] != 0)
            //{
            //    var newMomoInsert = new MomoInfoModel
            //    {
            //        OrderId = requestQuery["orderId"],
            //        FullName = User.FindFirstValue("email"),
            //        Amount = decimal.Parse(requestQuery["Amount"]),
            //        OrderInfo = requestQuery["orderInfo"],
            //        DatePaid = DateTime.Now,
            //    };
            //    // Do backend here later if can success momo payment
            //    _context.Add(newMomoInsert);
            //    await _context.SaveChangesAsync();

            //    // Tien hanh dat hang khi thanh toan momo thanh cong
            //    await Checkout("Momo", requestQuery["OrderId"]);
            //}
            //else
            //{
            //    TempData["success"] = "Giao dich Momo ko thanh cong.";
            //    return RedirectToAction("Index", "Cart");
            //}
            return View(response);
        }

        [HttpGet] // VnPay
        public async Task<IActionResult> PaymentCallbackVnpay()
        {
            var response = _vnPayService.PaymentExecute(Request.Query);
            //return Json(response);
            if (response.VnPayResponseCode == "00") // giao dich thanh cong
            {
                var newVnPayInsert = new VnpayForCreationDto() 
                {
                    OrderId = response.OrderId,
                    PaymentMethod = response.PaymentMethod,
                    OrderDescription = response.OrderDescription,
                    TransactionId = response.TransactionId,
                    PaymentId = response.PaymentId
                };

                using HttpResponseMessage createResponse = await _externalPaymentService.CreateVnpayAsync(newVnPayInsert);
                //response.EnsureSuccessStatusCode();
                if (createResponse.IsSuccessStatusCode) // can use httpResponseMessage.EnsureSuccessStatusCode(); to treat statusCode as error data response
                {
                    //var responseContent = await response.Content.ReadAsStringAsync();
                    //Console.WriteLine("UploadFileInModel_WithHttpClientAsync response :" + responseContent);

                    //TempData["success"] = "Add Vnpay successfully";
                    await Checkout(response.PaymentMethod, response.PaymentId);
                }
                else
                {
                    var errMsg = JsonConvert.DeserializeObject<dynamic>(createResponse.Content.ReadAsStringAsync().Result);
                    //TempData["error"] = errMsg; // if user back history to this url will get error here so don't show error here
                }
            }
            else
            {
                //TempData["success"] = "Vnpay payment successfully";
                return RedirectToAction("Index", "Cart");
            }
            return View(response);
        }
    }
}
