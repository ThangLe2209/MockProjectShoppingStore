using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using ShoppingStore.Client.Repository;
using ShoppingStore.Model.Dtos;

namespace ShoppingStore.Client.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")] // also check camelcase
    public class UserController : Controller
    {
        private readonly UserService _userService;
        private readonly RoleService _roleService;
        public UserController(UserService userService, RoleService roleService)
        {
            _userService = userService;
            _roleService = roleService;
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.Class = "/Admin/User";
            return View(await _userService.GetUsersAsync());
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            ViewBag.Class = "/Admin/User";
            ViewBag.UserRoles = new SelectList(await _roleService.GetRolesAsync(), "Id", "Value");
            ViewBag.Countries = new SelectList(
                new[] {
                    new { Id = "be", Value = "Belgium" },
                    new { Id = "us", Value = "United States of America" },
                    new { Id = "in", Value = "India" },
                    new { Id = "vi", Value = "VietNam" } },
                "Id",
                "Value");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UserForCreationDto user)
        {
            ViewBag.Class = "/Admin/User";
            ViewBag.UserRoles = new SelectList(await _roleService.GetRolesAsync(), "Id", "Value", user.UserRoleId);
            ViewBag.Countries = new SelectList(
                new[] {
                    new { Id = "be", Value = "Belgium" },
                    new { Id = "us", Value = "United States of America" },
                    new { Id = "in", Value = "India" },
                    new { Id = "vi", Value = "VietNam" } },
                "Id",
                "Value", user.Country);
            if (ModelState.IsValid)
            {

                using HttpResponseMessage response = await _userService.CreateUserAsync(user);
                //response.EnsureSuccessStatusCode();
                if (response.IsSuccessStatusCode) // can use httpResponseMessage.EnsureSuccessStatusCode(); to treat statusCode as error data response
                {
                    //var responseContent = await response.Content.ReadAsStringAsync();
                    //Console.WriteLine("UploadFileInModel_WithHttpClientAsync response :" + responseContent);
                    TempData["success"] = "Add User successfully";
                    return RedirectToAction("Index");
                }
                else
                {
                    var errMsg = JsonConvert.DeserializeObject<dynamic>(response.Content.ReadAsStringAsync().Result);
                    TempData["error"] = errMsg;
                    return View(user);
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
                return View(user);
            }
        }
        public async Task<IActionResult> ActiveAccount(Guid Id)
        {
            var user = await _userService.GetUserByIdAsync(Id);
            using HttpResponseMessage response = await _userService.ActiveUserAsync(user.SecurityCode);
            //response.EnsureSuccessStatusCode();
            if (response.IsSuccessStatusCode) // can use httpResponseMessage.EnsureSuccessStatusCode(); to treat statusCode as error data response
            {
                //var responseContent = await response.Content.ReadAsStringAsync();
                //Console.WriteLine("UploadFileInModel_WithHttpClientAsync response :" + responseContent);
                TempData["success"] = "Active User successfully";
                return RedirectToAction("Index");
            }
            else
            {
                var errMsg = JsonConvert.DeserializeObject<dynamic>(response.Content.ReadAsStringAsync().Result);
                TempData["error"] = errMsg;
                return RedirectToAction("Index");
            }
        }

        public async Task<IActionResult> Delete(Guid Id)
        {
            using HttpResponseMessage response = await _userService.DeleteUserAsync(Id);
            //response.EnsureSuccessStatusCode();
            if (response.IsSuccessStatusCode) // can use httpResponseMessage.EnsureSuccessStatusCode(); to treat statusCode as error data response
            {
                //var responseContent = await response.Content.ReadAsStringAsync();
                //Console.WriteLine("UploadFileInModel_WithHttpClientAsync response :" + responseContent);
                TempData["success"] = "Remove User successfully";
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
            ViewBag.Class = "/Admin/User";
            UserDto user = await _userService.GetUserByIdAsync(Id, "country");
            ViewBag.UserRoles = new SelectList(await _roleService.GetRolesAsync(), "Id", "Value", user.UserRoleId);
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
            return View(userEdit);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid Id, UserForEditDto updatedUser)
        {
            ViewBag.Class = "/Admin/User";
            ViewBag.UserRoles = new SelectList(await _roleService.GetRolesAsync(), "Id", "Value", updatedUser.UserRoleId);
            ViewBag.Countries = new SelectList(
                new[] {
                    new { Id = "be", Value = "Belgium" },
                    new { Id = "us", Value = "United States of America" },
                    new { Id = "in", Value = "India" },
                    new { Id = "vi", Value = "VietNam" } },
                "Id",
                "Value", updatedUser.Country);

            if (ModelState.IsValid)
            {
                //add data to db
                //TempData["success"] = "Model da ok het";
                using HttpResponseMessage response = await _userService.UpdateUserAsync(Id, updatedUser);
                //response.EnsureSuccessStatusCode();
                if (response.IsSuccessStatusCode) // can use httpResponseMessage.EnsureSuccessStatusCode(); to treat statusCode as error data response
                {
                    //var responseContent = await response.Content.ReadAsStringAsync();
                    //Console.WriteLine("UploadFileInModel_WithHttpClientAsync response :" + responseContent);
                    TempData["success"] = "Update User successfully";
                    return RedirectToAction("Index");
                }
                else
                {
                    var errMsg = JsonConvert.DeserializeObject<dynamic>(response.Content.ReadAsStringAsync().Result);
                    TempData["error"] = errMsg;
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
                return View(updatedUser);
            }
        }
    }
}
