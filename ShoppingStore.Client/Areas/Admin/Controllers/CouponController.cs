using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ShoppingStore.Client.Repository;
using ShoppingStore.Model.Dtos;

namespace ShoppingStore.Client.Areas.Admin.Controllers
{
	[Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class CouponController : Controller
	{
		private readonly CouponService _couponService;

		public CouponController(CouponService couponService)
		{
			_couponService = couponService;
		}
		public async Task<IActionResult> Index(CouponForCreationDto coupon)
		{
            ViewBag.Class = "/Admin/Coupon";
            var coupon_list = await _couponService.GetCouponsAsync();
            ViewBag.Coupons = coupon_list;
            return View(coupon);
		}

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CouponForCreationDto coupon)
        {
            ViewBag.Class = "/Admin/Coupon";
            if (ModelState.IsValid)
            {
                using HttpResponseMessage response = await _couponService.CreateCouponAsync(coupon);
                //response.EnsureSuccessStatusCode();
                if (response.IsSuccessStatusCode) // can use httpResponseMessage.EnsureSuccessStatusCode(); to treat statusCode as error data response
                {
                    //var responseContent = await response.Content.ReadAsStringAsync();
                    //Console.WriteLine("UploadFileInModel_WithHttpClientAsync response :" + responseContent);
                    TempData["success"] = "Add Coupon successfully";
                    return RedirectToAction("Index");
                }
                else
                {
                    var errMsg = JsonConvert.DeserializeObject<dynamic>(response.Content.ReadAsStringAsync().Result);
                    TempData["error"] = errMsg;
                    return RedirectToAction("Index","Coupon",coupon);
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
                return RedirectToAction("Index", "Coupon", coupon);
            }
        }

        public async Task<IActionResult> Delete(Guid Id)
        {
            //var response1 = await _shoppingStoreService.GetCategoryByIdAsync(Id);
            //TempData["success"] = response1;
            //return View();
            using HttpResponseMessage response = await _couponService.DeleteCouponAsync(Id);
            //response.EnsureSuccessStatusCode();
            if (response.IsSuccessStatusCode) // can use httpResponseMessage.EnsureSuccessStatusCode(); to treat statusCode as error data response
            {
                //var responseContent = await response.Content.ReadAsStringAsync();
                //Console.WriteLine("UploadFileInModel_WithHttpClientAsync response :" + responseContent);
                TempData["success"] = "Remove Coupon successfully";
                return RedirectToAction("Index");
            }
            else
            {
                var errMsg = JsonConvert.DeserializeObject<dynamic>(response.Content.ReadAsStringAsync().Result);
                TempData["error"] = errMsg;
                return RedirectToAction("Index");
            }
        }

        public async Task<IActionResult> Edit(Guid Id)
        {
            ViewBag.Class = "/Admin/Coupon";
            CouponDto coupon = await _couponService.GetCouponByIdAsync(Id);
            //CouponForEditDto couponEdit = new CouponForEditDto()
            //{
            //    Name = brand.Name,
            //    Description = brand.Description,
            //    Slug = brand.Slug,
            //    Status = brand.Status
            //};
            var jsonParent = JsonConvert.SerializeObject(coupon);
            CouponForEditDto couponEdit = JsonConvert.DeserializeObject<CouponForEditDto>(jsonParent);
            return View(couponEdit);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid Id, CouponForEditDto coupon)
        {
            ViewBag.Class = "/Admin/Coupon";
            if (ModelState.IsValid)
            {
                //add data to db
                //TempData["success"] = "Model da ok het";
                using HttpResponseMessage response = await _couponService.UpdateCouponAsync(Id, coupon);
                //response.EnsureSuccessStatusCode();
                if (response.IsSuccessStatusCode) // can use httpResponseMessage.EnsureSuccessStatusCode(); to treat statusCode as error data response
                {
                    //var responseContent = await response.Content.ReadAsStringAsync();
                    //Console.WriteLine("UploadFileInModel_WithHttpClientAsync response :" + responseContent);
                    TempData["success"] = "Update Coupon successfully";
                    return RedirectToAction("Index");
                }
                else
                {
                    var errMsg = JsonConvert.DeserializeObject<dynamic>(response.Content.ReadAsStringAsync().Result);
                    TempData["error"] = errMsg;
                    return View(coupon);
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
                return View(coupon);
            }
        }
    }
}
