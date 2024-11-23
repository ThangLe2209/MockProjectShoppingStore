using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ShoppingStore.Areas.Admin.Repository;
using ShoppingStore.Client.Repository;
using ShoppingStore.Model;
using ShoppingStore.Model.Dtos;


namespace ShoppingStore.Client.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    //[Authorize]
    public class CategoryController : Controller
    {
        private readonly ProductService _productService;
        private readonly CategoryService _categoryService;
        private readonly BrandService _brandService;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public CategoryController(ProductService productService, IWebHostEnvironment webHostEnvironment, CategoryService categoryService, BrandService brandService)
        {
            _productService = productService;
            _categoryService = categoryService;
            _brandService = brandService;
            _webHostEnvironment = webHostEnvironment;
        }

        //[Route("Index")]
        public async Task<IActionResult> Index(int pg = 1)
        {
            ViewBag.Class = "/Admin/Category";
            List<CategoryDto> category = (await _categoryService.GetCategoriesAsync()) as List<CategoryDto>; //33 datas


            //const int pageSize = 10; //10 items/trang

            //if (pg < 1) //page < 1;
            //{
            //    pg = 1; //page ==1
            //}
            //int recsCount = category.Count(); //33 items;

            //var pager = new Paginate(recsCount, pg, pageSize);

            //int recSkip = (pg - 1) * pageSize; //(3 - 1) * 10; 

            //category.Skip(20).Take(10).ToList();

            //var data = category.Skip(recSkip).Take(pager.PageSize).ToList();

            //ViewBag.Pager = pager;

            //return View(data);
            return View(category);
        }

        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.Class = "/Admin/Category";
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CategoryForCreationDto category)
        {
            ViewBag.Class = "/Admin/Category";
            if (ModelState.IsValid)
            {
                using HttpResponseMessage response = await _categoryService.CreateCategoryAsync(category);
                //response.EnsureSuccessStatusCode();
                if (response.IsSuccessStatusCode) // can use httpResponseMessage.EnsureSuccessStatusCode(); to treat statusCode as error data response
                {
                    //var responseContent = await response.Content.ReadAsStringAsync();
                    //Console.WriteLine("UploadFileInModel_WithHttpClientAsync response :" + responseContent);
                    TempData["success"] = "Add Category successfully";
                    return RedirectToAction("Index");
                }
                else
                {
                    var errMsg = JsonConvert.DeserializeObject<dynamic>(response.Content.ReadAsStringAsync().Result);
                    TempData["error"] = errMsg;
                    return View(category);
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
                return View(category);
            }
        }

        public async Task<IActionResult> Delete(Guid Id)
        {
            //var response1 = await _shoppingStoreService.GetCategoryByIdAsync(Id);
            //TempData["success"] = response1;
            //return View();
            using HttpResponseMessage response = await _categoryService.DeleteCategoryAsync(Id);
            //response.EnsureSuccessStatusCode();
            if (response.IsSuccessStatusCode) // can use httpResponseMessage.EnsureSuccessStatusCode(); to treat statusCode as error data response
            {
                //var responseContent = await response.Content.ReadAsStringAsync();
                //Console.WriteLine("UploadFileInModel_WithHttpClientAsync response :" + responseContent);
                TempData["success"] = "Remove Category successfully";
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
            ViewBag.Class = "/Admin/Category";
            CategoryDto category = await _categoryService.GetCategoryByIdAsync(Id);
            var jsonParent = JsonConvert.SerializeObject(category);
            CategoryForEditDto categoryEdit = JsonConvert.DeserializeObject<CategoryForEditDto>(jsonParent);
            return View(categoryEdit);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid Id, CategoryForEditDto category)
        {
            ViewBag.Class = "/Admin/Category";
            if (ModelState.IsValid)
            {
                //add data to db
                //TempData["success"] = "Model da ok het";
                using HttpResponseMessage response = await _categoryService.UpdateCategoryAsync(Id, category);
                //response.EnsureSuccessStatusCode();
                if (response.IsSuccessStatusCode) // can use httpResponseMessage.EnsureSuccessStatusCode(); to treat statusCode as error data response
                {
                    //var responseContent = await response.Content.ReadAsStringAsync();
                    //Console.WriteLine("UploadFileInModel_WithHttpClientAsync response :" + responseContent);
                    TempData["success"] = "Update Category successfully";
                    return RedirectToAction("Index");
                }
                else
                {
                    var errMsg = JsonConvert.DeserializeObject<dynamic>(response.Content.ReadAsStringAsync().Result);
                    TempData["error"] = errMsg;
                    return View(category);
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
                return View(category);
            }
        }
    }
}
