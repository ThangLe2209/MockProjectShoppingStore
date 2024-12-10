using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ShoppingStore.Client.Repository;
using ShoppingStore.Client.Services.Momo;
using ShoppingStore.Client.Services.Vnpay;
using ShoppingStore.Model;
using ShoppingStore.Model.Vnpay;
using ShoppingStore.Models;

namespace ShoppingStore.Client.Controllers
{
    public class PaymentController : Controller
    {
        private readonly IMomoService _momoService;
        private readonly IVnPayService _vnPayService;
        private readonly ExternalPaymentService _externalPaymentService;
        public PaymentController(IMomoService momoService, IVnPayService vnPayService, ExternalPaymentService externalPaymentService)
        {
            _momoService = momoService;
            _vnPayService = vnPayService;
            _externalPaymentService = externalPaymentService;
        }
        [HttpPost]
        [Route("CreatePaymentUrl")]
        public async Task<IActionResult> CreatePaymentMomo(OrderInfoModel model)
        {
            model.Amount = model.Amount * 22000; // here is USD change to VND
            var response = await _momoService.CreatePaymentAsync(model);
            return Redirect(response.PayUrl);
        }

        public async Task<IActionResult> CreatePaymentUrlVnpay(PaymentInformationModel model)
        {
            if (Request.Cookies["ShippingCity"] == null || Request.Cookies["ShippingDistrict"] == null
                || Request.Cookies["ShippingWard"] == null)
            {
                TempData["error"] = "Please calculate shipping cost first!";
                return RedirectToAction("Index", "Cart");
            }

            // check valid Price(devtools browser sửa input hidden amount)
            List<CartItemModel> cartItems = HttpContext.Session.GetJson<List<CartItemModel>>("Cart") ?? new List<CartItemModel>();
            var rawTotal = cartItems.Sum(x => x.Quantity * x.Price);
            
            using HttpResponseMessage checkResponse = await _externalPaymentService.CheckValidPaymentPrice(rawTotal, Request?.Cookies["CouponName"], Request.Cookies["ShippingPrice"] ?? "", model.Amount.ToString()); // Amount is double here we treat as string so if it's not number then default double is 0 => check BE Condition is "0" (see in BE file)
            if (!checkResponse.IsSuccessStatusCode)
            {
                var errMsg = JsonConvert.DeserializeObject<dynamic>(checkResponse.Content.ReadAsStringAsync().Result);
                TempData["error"] = errMsg;
                return RedirectToAction("Index", "Cart");
            }

            // Create PaymentUrl
            var url = _vnPayService.CreatePaymentUrl(model, HttpContext);

            //if (Request.Cookies["CouponName"] != null) 
            //{
            //    url = $"{url}&CouponName={Request.Cookies["CouponName"]}"; // Cannot do this, vnpay not accept custom header (or need configure more not just like this but not research yet)
            //}
            return Redirect(url);
        }
    }
}
