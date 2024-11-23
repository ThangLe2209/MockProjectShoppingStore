using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using ShoppingStore.Client.Repository;
using ShoppingStore.Model;
using ShoppingStore.Model.Dtos;
using System.Net.Http.Headers;

namespace ShoppingStore.Client.Areas.Admin.Controllers
{
	[Area("Admin")]
    [Authorize(Roles = "Admin")]
    //[Authorize(Roles = "Publisher,Author,Admin")]
    public class SliderController : Controller
	{
		private readonly SliderService _sliderService;
		public SliderController(SliderService sliderService)
		{
			_sliderService = sliderService;
		}
		public async Task<IActionResult> Index()
		{
			ViewBag.Class = "/Admin/Slider";
            var result = await _sliderService.GetSlidersAsync();
			return View(result);
		}

        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.Class = "/Admin/Slider";
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SliderForCreationDto slider)
        {
            ViewBag.Class = "/Admin/Slider";
            if (ModelState.IsValid)
            {
                using (var multipartFormContent = new MultipartFormDataContent())
                {
                    multipartFormContent.Add(new StringContent(slider.Name), "Name");
                    multipartFormContent.Add(new StringContent(slider.Description ?? ""), "Description");
                    multipartFormContent.Add(new StringContent(slider.Status.ToString() ?? "1"), "Status");
                    //multipartFormContent.Add(new StringContent(slider.Image ?? ""), "Image");
                    multipartFormContent.Add(new StringContent(slider.ImageUrl == null ? "" : slider.ImageUrl.ToString()), "ImageUrl");

                    // Add the file
                    if (slider.ImageUpload != null)
                    {
                        var fileStreamContent = new StreamContent(slider.ImageUpload.OpenReadStream());
                        fileStreamContent.Headers.ContentType = MediaTypeHeaderValue.Parse(slider.ImageUpload.ContentType);
                        multipartFormContent.Add(fileStreamContent, name: "ImageUpload", slider.ImageUpload.FileName);
                    }
                    using var response = await _sliderService.CreateSliderAsync(multipartFormContent);
                    //response.EnsureSuccessStatusCode();
                    if (response.IsSuccessStatusCode) // can use httpResponseMessage.EnsureSuccessStatusCode(); to treat statusCode as error data response
                    {
                        //var responseContent = await response.Content.ReadAsStringAsync();
                        //Console.WriteLine("UploadFileInModel_WithHttpClientAsync response :" + responseContent);
                        TempData["success"] = "Add Slider successfully";
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        var errMsg = JsonConvert.DeserializeObject<dynamic>(response.Content.ReadAsStringAsync().Result);
                        TempData["error"] = errMsg;
                        return View(slider);
                    }
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
                return View(slider);
            }
        }

        public async Task<IActionResult> Edit(Guid Id)
        {
            ViewBag.Class = "/Admin/Slider";
            SliderDto slider = await _sliderService.GetSliderByIdAsync(Id);
            var jsonParent = JsonConvert.SerializeObject(slider);
            SliderForEditDto sliderEdit = JsonConvert.DeserializeObject<SliderForEditDto>(jsonParent);
            return View(sliderEdit);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid Id, SliderForEditDto slider)
        {
            ViewBag.Class = "/Admin/Slider";
            if (ModelState.IsValid)
            {
                //add data to db
                //TempData["success"] = "Model da ok het";
                using (var multipartFormContent = new MultipartFormDataContent())
                {
                    multipartFormContent.Add(new StringContent(slider.Name), "Name");
                    multipartFormContent.Add(new StringContent(slider.Description ?? ""), "Description");
                    multipartFormContent.Add(new StringContent(slider.Status.ToString() ?? "1"), "Status");
                    //multipartFormContent.Add(new StringContent(slider.Image ?? ""), "Image");
                    multipartFormContent.Add(new StringContent(slider.ImageUrl == null ? "" : slider.ImageUrl.ToString()), "ImageUrl");

                    // Add the file
                    if (slider.ImageUpload != null)
                    {
                        var fileStreamContent = new StreamContent(slider.ImageUpload.OpenReadStream());
                        fileStreamContent.Headers.ContentType = MediaTypeHeaderValue.Parse(slider.ImageUpload.ContentType);
                        multipartFormContent.Add(fileStreamContent, name: "ImageUpload", slider.ImageUpload.FileName);
                    }
                    using var response = await _sliderService.UpdateSliderAsync(Id, multipartFormContent);
                    //response.EnsureSuccessStatusCode();
                    if (response.IsSuccessStatusCode) // can use httpResponseMessage.EnsureSuccessStatusCode(); to treat statusCode as error data response
                    {
                        //var responseContent = await response.Content.ReadAsStringAsync();
                        //Console.WriteLine("UploadFileInModel_WithHttpClientAsync response :" + responseContent);
                        TempData["success"] = "Update Slider successfully";
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        var errMsg = JsonConvert.DeserializeObject<dynamic>(response.Content.ReadAsStringAsync().Result);
                        TempData["error"] = errMsg;
                        return View(slider);
                    }
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
                return View(slider);
            }
        }

        public async Task<IActionResult> Delete(Guid Id)
        {
            using HttpResponseMessage response = await _sliderService.DeleteSliderAsync(Id);
            var callbackSuccess = (Stream contentStream) =>
            {
                //var responseContent = await response.Content.ReadAsStringAsync();
                //Console.WriteLine("UploadFileInModel_WithHttpClientAsync response :" + responseContent);
                TempData["success"] = "Delete Slider successfully";
                return RedirectToAction("Index");
            };
            var callbackFail = (string errMsg) =>
            {
                TempData["error"] = errMsg;
                return RedirectToAction("Index");
            };
            return await HandleResponseResult.HandleResultWithNoResponseData(response, callbackSuccess, callbackFail);
        }
    }
}
