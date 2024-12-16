using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ShoppingStore.Client.Repository;
using ShoppingStore.Model;
using System.Net.Http.Headers;

namespace ShoppingStore.Client.Areas.Admin.Controllers
{
    [Area("Admin")]
    //[Authorize(Roles = "Admin")]
    public class SongController : Controller
    {
        private readonly SongService _songService;
        public SongController(SongService songService)
        {
            _songService = songService;
        }
        public async Task<IActionResult> Index()
        {
            ViewBag.Class = "/Admin/Song";
            var result = await _songService.GetSongsAsync();
            return View(result);
        }

        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.Class = "/Admin/Song";
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SongForCreationDto song)
        {
            ViewBag.Class = "/Admin/Song";

            if (ModelState.IsValid)
            {
                using (var multipartFormContent = new MultipartFormDataContent())
                {
                    multipartFormContent.Add(new StringContent(song.Name), "Name");
                    multipartFormContent.Add(new StringContent(song.Description ?? ""), "Description");
                    multipartFormContent.Add(new StringContent(song.Status.ToString() ?? "1"), "Status");
                    //multipartFormContent.Add(new StringContent(slider.Image ?? ""), "Image");
                    multipartFormContent.Add(new StringContent(song.ImageUrl == null ? "" : song.ImageUrl.ToString()), "ImageUrl");

                    // Add the file
                    if (song.ImageUpload != null)
                    {
                        var fileStreamContent = new StreamContent(song.ImageUpload.OpenReadStream());
                        fileStreamContent.Headers.ContentType = MediaTypeHeaderValue.Parse(song.ImageUpload.ContentType);
                        multipartFormContent.Add(fileStreamContent, name: "ImageUpload", song.ImageUpload.FileName);
                    }

                    var songfileStreamContent = new StreamContent(song.SongUpload.OpenReadStream());
                    songfileStreamContent.Headers.ContentType = MediaTypeHeaderValue.Parse(song.SongUpload.ContentType);
                    multipartFormContent.Add(songfileStreamContent, name: "SongUpload", song.SongUpload.FileName);

                    using var response = await _songService.CreateSongAsync(multipartFormContent);
                    //response.EnsureSuccessStatusCode();
                    if (response.IsSuccessStatusCode) // can use httpResponseMessage.EnsureSuccessStatusCode(); to treat statusCode as error data response
                    {
                        //var responseContent = await response.Content.ReadAsStringAsync();
                        //Console.WriteLine("UploadFileInModel_WithHttpClientAsync response :" + responseContent);
                        TempData["success"] = "Add Song successfully";
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        var errMsg = JsonConvert.DeserializeObject<dynamic>(response.Content.ReadAsStringAsync().Result);
                        TempData["error"] = errMsg;
                        return View(song);
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
                return View(song);
            }
        }

        public async Task<IActionResult> Edit(Guid Id)
        {
            ViewBag.Class = "/Admin/Song";
            SongDto song = await _songService.GetSongByIdAsync(Id);
            var jsonParent = JsonConvert.SerializeObject(song);
            SongForEditDto songEdit = JsonConvert.DeserializeObject<SongForEditDto>(jsonParent);
            return View(songEdit);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid Id, SongForEditDto song)
        {
            ViewBag.Class = "/Admin/Slider";
            if (ModelState.IsValid)
            {
                //add data to db
                //TempData["success"] = "Model da ok het";
                using (var multipartFormContent = new MultipartFormDataContent())
                {
                    multipartFormContent.Add(new StringContent(song.Name), "Name");
                    multipartFormContent.Add(new StringContent(song.Description ?? ""), "Description");
                    multipartFormContent.Add(new StringContent(song.Status.ToString() ?? "1"), "Status");
                    //multipartFormContent.Add(new StringContent(slider.Image ?? ""), "Image");
                    multipartFormContent.Add(new StringContent(song.ImageUrl == null ? "" : song.ImageUrl.ToString()), "ImageUrl");

                    // Add the file
                    if (song.ImageUpload != null)
                    {
                        var fileStreamContent = new StreamContent(song.ImageUpload.OpenReadStream());
                        fileStreamContent.Headers.ContentType = MediaTypeHeaderValue.Parse(song.ImageUpload.ContentType);
                        multipartFormContent.Add(fileStreamContent, name: "ImageUpload", song.ImageUpload.FileName);
                    }

                    if (song.SongUpload != null)
                    {
                        var songfileStreamContent = new StreamContent(song.SongUpload.OpenReadStream());
                        songfileStreamContent.Headers.ContentType = MediaTypeHeaderValue.Parse(song.SongUpload.ContentType);
                        multipartFormContent.Add(songfileStreamContent, name: "SongUpload", song.SongUpload.FileName);
                    }

                    using var response = await _songService.UpdateSongAsync(Id, multipartFormContent);
                    //response.EnsureSuccessStatusCode();
                    if (response.IsSuccessStatusCode) // can use httpResponseMessage.EnsureSuccessStatusCode(); to treat statusCode as error data response
                    {
                        //var responseContent = await response.Content.ReadAsStringAsync();
                        //Console.WriteLine("UploadFileInModel_WithHttpClientAsync response :" + responseContent);
                        TempData["success"] = "Update Song successfully";
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        var errMsg = JsonConvert.DeserializeObject<dynamic>(response.Content.ReadAsStringAsync().Result);
                        TempData["error"] = errMsg;
                        return View(song);
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
                return View(song);
            }
        }

        public async Task<IActionResult> Delete(Guid Id)
        {
            using HttpResponseMessage response = await _songService.DeleteSongAsync(Id);
            var callbackSuccess = (Stream contentStream) =>
            {
                //var responseContent = await response.Content.ReadAsStringAsync();
                //Console.WriteLine("UploadFileInModel_WithHttpClientAsync response :" + responseContent);
                TempData["success"] = "Delete Song successfully";
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
