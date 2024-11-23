using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ShoppingStore.Client.Repository;
using ShoppingStore.Model;
using ShoppingStore.Model.Dtos;
using System.Net.Http.Headers;

namespace ShoppingStore.Client.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class ContactController : Controller
    {
        private readonly ContactService _contactService;
        public ContactController(ContactService contactService)
        {
            _contactService = contactService;
        }
        public async Task<IActionResult> Index()
        {
            ViewBag.Class = "/Admin/Contact";
            var result = await _contactService.GetContactsAsync();
            return View(result);
        }

        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.Class = "/Admin/Contact";
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ContactForCreationDto contact)
        {
            ViewBag.Class = "/Admin/Contact";
            if (ModelState.IsValid)
            {
                using (var multipartFormContent = new MultipartFormDataContent())
                {
                    multipartFormContent.Add(new StringContent(contact.Name), "Name");
                    multipartFormContent.Add(new StringContent(contact.Map), "Map");
                    multipartFormContent.Add(new StringContent(contact.Email), "Email");
                    multipartFormContent.Add(new StringContent(contact.Phone), "Phone");
                    multipartFormContent.Add(new StringContent(contact.Description ?? ""), "Description");
                    //multipartFormContent.Add(new StringContent(contact.LogoImg ?? ""), "LogoImg");
                    multipartFormContent.Add(new StringContent(contact.ImageUrl == null ? "" : contact.ImageUrl.ToString()), "ImageUrl");

                    // Add the file
                    if (contact.ImageUpload != null)
                    {
                        var fileStreamContent = new StreamContent(contact.ImageUpload.OpenReadStream());
                        fileStreamContent.Headers.ContentType = MediaTypeHeaderValue.Parse(contact.ImageUpload.ContentType);
                        multipartFormContent.Add(fileStreamContent, name: "ImageUpload", contact.ImageUpload.FileName);
                    }
                    using var response = await _contactService.CreateContactAsync(multipartFormContent);
                    //response.EnsureSuccessStatusCode();
                    if (response.IsSuccessStatusCode) // can use httpResponseMessage.EnsureSuccessStatusCode(); to treat statusCode as error data response
                    {
                        //var responseContent = await response.Content.ReadAsStringAsync();
                        //Console.WriteLine("UploadFileInModel_WithHttpClientAsync response :" + responseContent);
                        TempData["success"] = "Add Contact successfully";
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        var errMsg = JsonConvert.DeserializeObject<dynamic>(response.Content.ReadAsStringAsync().Result);
                        TempData["error"] = errMsg;
                        return View(contact);
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
                return View(contact);
            }
        }

        public async Task<IActionResult> Edit(Guid Id)
        {
            ViewBag.Class = "/Admin/Contact";
            ContactDto contact = await _contactService.GetContactByIdAsync(Id);
            var jsonParent = JsonConvert.SerializeObject(contact);
            ContactForEditDto contactEdit = JsonConvert.DeserializeObject<ContactForEditDto>(jsonParent);
            return View(contactEdit);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid Id, ContactForEditDto contact)
        {
            ViewBag.Class = "/Admin/Contact";
            if (ModelState.IsValid)
            {
                //add data to db
                //TempData["success"] = "Model da ok het";
                using (var multipartFormContent = new MultipartFormDataContent())
                {
                    multipartFormContent.Add(new StringContent(contact.Name), "Name");
                    multipartFormContent.Add(new StringContent(contact.Map), "Map");
                    multipartFormContent.Add(new StringContent(contact.Email), "Email");
                    multipartFormContent.Add(new StringContent(contact.Phone), "Phone");
                    multipartFormContent.Add(new StringContent(contact.Description ?? ""), "Description");
                    //multipartFormContent.Add(new StringContent(contact.LogoImg ?? ""), "LogoImg");
                    multipartFormContent.Add(new StringContent(contact.ImageUrl == null ? "" : contact.ImageUrl.ToString()), "ImageUrl");

                    // Add the file
                    if (contact.ImageUpload != null)
                    {
                        var fileStreamContent = new StreamContent(contact.ImageUpload.OpenReadStream());
                        fileStreamContent.Headers.ContentType = MediaTypeHeaderValue.Parse(contact.ImageUpload.ContentType);
                        multipartFormContent.Add(fileStreamContent, name: "ImageUpload", contact.ImageUpload.FileName);
                    }
                    using var response = await _contactService.UpdateContactAsync(Id, multipartFormContent);
                    //response.EnsureSuccessStatusCode();
                    if (response.IsSuccessStatusCode) // can use httpResponseMessage.EnsureSuccessStatusCode(); to treat statusCode as error data response
                    {
                        //var responseContent = await response.Content.ReadAsStringAsync();
                        //Console.WriteLine("UploadFileInModel_WithHttpClientAsync response :" + responseContent);
                        TempData["success"] = "Update Contact successfully";
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        var errMsg = JsonConvert.DeserializeObject<dynamic>(response.Content.ReadAsStringAsync().Result);
                        TempData["error"] = errMsg;
                        return View(contact);
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
                return View(contact);
            }
        }

        public async Task<IActionResult> Delete(Guid Id)
        {
            using HttpResponseMessage response = await _contactService.DeleteContactAsync(Id);
            var callbackSuccess = (Stream contentStream) =>
            {
                //var responseContent = await response.Content.ReadAsStringAsync();
                //Console.WriteLine("UploadFileInModel_WithHttpClientAsync response :" + responseContent);
                TempData["success"] = "Delete Contact successfully";
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
