using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ShoppingStore.Client.Repository;

namespace ShoppingStore.Client.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    //[Authorize]
    public class OrderController: Controller
    {
        private readonly OrderService _orderService;
        private readonly OrderDetailService _orderDetailService;
        private readonly ExternalPaymentService _externalPaymentService;
        private readonly CouponService _couponService;

        public OrderController(OrderService orderService, OrderDetailService orderDetailService, ExternalPaymentService externalPaymentService, CouponService couponService)
        {
            _orderService = orderService;
            _orderDetailService = orderDetailService;
            _externalPaymentService = externalPaymentService;
            _couponService = couponService;
        }
        public async Task<IActionResult> Index()
        {
            ViewBag.Class = "/Admin/Order";
            return View(await _orderService.GetOrdersAsync());
        }

        public async Task<IActionResult> ViewOrder(string ordercode)
        {
            try
            {
                ViewBag.Class = "/Admin/Order";
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
                return RedirectToAction("Index","Order");
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateOrder(string ordercode, int status)
        {
            ViewBag.Class = "/Admin/Order";
            using HttpResponseMessage response = await _orderService.UpdateOrderAsync(ordercode, status);
            //response.EnsureSuccessStatusCode();
            if (response.IsSuccessStatusCode) // can use httpResponseMessage.EnsureSuccessStatusCode(); to treat statusCode as error data response
            {
                //var responseContent = await response.Content.ReadAsStringAsync();
                //Console.WriteLine("UploadFileInModel_WithHttpClientAsync response :" + responseContent);
                TempData["success"] = "Update Order successfully";
                return Ok(new { success = true, message = "Order status updated successfully", redirectToUrl = Url.Action("Index", "Order") });
            }
            else
            {
                var errMsg = JsonConvert.DeserializeObject<dynamic>(response.Content.ReadAsStringAsync().Result);
                TempData["error"] = errMsg;
                return StatusCode(500, new { message = errMsg, redirectToUrl = Url.Action("Index", "Order") });
                //return RedirectToAction("Index");
            }
        }

        public async Task<IActionResult> Delete(Guid Id)
        {
            //var response1 = await _shoppingStoreService.GetCategoryByIdAsync(Id);
            //TempData["success"] = response1;
            //return View();
            using HttpResponseMessage response = await _orderService.DeleteOrderAsync(Id);
            //response.EnsureSuccessStatusCode();
            if (response.IsSuccessStatusCode) // can use httpResponseMessage.EnsureSuccessStatusCode(); to treat statusCode as error data response
            {
                //var responseContent = await response.Content.ReadAsStringAsync();
                //Console.WriteLine("UploadFileInModel_WithHttpClientAsync response :" + responseContent);
                TempData["success"] = "Remove Order successfully";
                return RedirectToAction("Index");
            }
            else
            {
                var errMsg = JsonConvert.DeserializeObject<dynamic>(response.Content.ReadAsStringAsync().Result);
                TempData["error"] = errMsg;
                return RedirectToAction("Index");
            }
        }

        //public async Task<IActionResult> PaymentMomoInfo(string orderId)
        //{
        //    // do backend later
        //    var momoInfo = await _context.MomoInfos.FirstOrDefaultAsync(m => m.OrderId == orderId);
        //    if (momoInfo == null)
        //    {
        //        return NotFound();
        //    }
        //    return View(momoInfo);
        //}

        public async Task<IActionResult> PaymentVnpayInfo(string paymentId)
        {
            ViewBag.Class = "/Admin/Order";
            var vnpayInfo = await _externalPaymentService.GetVnPayByPaymentIdAsync(paymentId);
            return View(vnpayInfo);
        }
    }
}
