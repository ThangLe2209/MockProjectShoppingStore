using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ShoppingStore.Client.Repository;
using ShoppingStore.Model;
using ShoppingStore.Model.Dtos;

namespace ShoppingStore.Client.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class ShippingController : Controller
    {
        private readonly ShippingService _shippingService;
        public ShippingController(ShippingService shippingService)
        {
            _shippingService = shippingService;
        }
        public async Task<IActionResult> Index()
        {
            ViewBag.Class = "/Admin/Shipping";
            var shippingList = await _shippingService.GetShippingsAsync();
            ViewBag.Shippings = shippingList;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken] // https://stackoverflow.com/questions/14473597/include-antiforgerytoken-in-ajax-post-asp-net-mvc
        public async Task<IActionResult> StoreShipping(ShippingForCreationDto shippingModel, string tinh, string quan, string phuong, decimal price)
        {
            // shippingModel in argument not get from view the asp.net core just mapping the field price (data from view) to the shippingModel(like created a new one object)
            // => C# just mapping the field we pass from the view that match the argument field object.
            var createShippingModal = new ShippingForCreationDto()
            {
                City = tinh,
                District = quan,
                Ward = phuong,
                Price = price
            };

            using HttpResponseMessage response = await _shippingService.CreateShippingAsync(createShippingModal);
            if (response.IsSuccessStatusCode) // can use httpResponseMessage.EnsureSuccessStatusCode(); to treat statusCode as error data response
            {
                TempData["success"] = "Add Shipping successfully";
                return Ok(new { success = true, message = "Add Shipping successfully", redirectToUrl = Url.Action("Index", "Shipping") });
            }
            else
            {
                var errMsg = JsonConvert.DeserializeObject<dynamic>(response.Content.ReadAsStringAsync().Result);
                bool? isDuplicate = errMsg?.duplicate;
                if (isDuplicate.HasValue && isDuplicate == true) 
                {
                    string? errMsgDuplicate = errMsg?.message;
                    //TempData["error"] = errMsgDuplicate;
                    return StatusCode(400, new { isDuplicate= true, message = errMsgDuplicate, redirectToUrl = Url.Action("Index", "Shipping") });
                }
                TempData["error"] = errMsg;
                return StatusCode(400, new { message = errMsg, redirectToUrl = Url.Action("Index", "Shipping") });
            }
        }

        public async Task<IActionResult> Delete(Guid Id)
        {
            //var response1 = await _shoppingStoreService.GetCategoryByIdAsync(Id);
            //TempData["success"] = response1;
            //return View();
            using HttpResponseMessage response = await _shippingService.DeleteShippingAsync(Id);
            //response.EnsureSuccessStatusCode();
            if (response.IsSuccessStatusCode) // can use httpResponseMessage.EnsureSuccessStatusCode(); to treat statusCode as error data response
            {
                //var responseContent = await response.Content.ReadAsStringAsync();
                //Console.WriteLine("UploadFileInModel_WithHttpClientAsync response :" + responseContent);
                TempData["success"] = "Remove Shipping successfully";
                return RedirectToAction("Index");
            }
            else
            {
                var errMsg = JsonConvert.DeserializeObject<dynamic>(response.Content.ReadAsStringAsync().Result);
                TempData["error"] = errMsg;
                return RedirectToAction("Index");
            }
        }
    }
}
