using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ShoppingStore.Client.Repository;
using ShoppingStore.Model.Dtos;

namespace ShoppingStore.Client.Areas.Admin.Controllers
{
	[Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class BrandController : Controller
	{
		private readonly ProductService _productService;
		private readonly CategoryService _categoryService;
		private readonly BrandService _brandService;
		private readonly IWebHostEnvironment _webHostEnvironment;
		public BrandController(ProductService productService, IWebHostEnvironment webHostEnvironment, CategoryService categoryService, BrandService brandService)
		{
			_productService = productService;
			_categoryService = categoryService;
			_brandService = brandService;
			_webHostEnvironment = webHostEnvironment;
		}
		public async Task<IActionResult> Index()
		{
			ViewBag.Class = "/Admin/Brand";
			return View(await _brandService.GetBrandsAsync());
		}

		[HttpGet]
		public IActionResult Create()
		{
			ViewBag.Class = "/Admin/Brand";
			return View();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create(BrandForCreationDto brand)
		{
            ViewBag.Class = "/Admin/Brand";
            if (ModelState.IsValid)
			{
				using HttpResponseMessage response = await _brandService.CreateBrandAsync(brand);
				//response.EnsureSuccessStatusCode();
				if (response.IsSuccessStatusCode) // can use httpResponseMessage.EnsureSuccessStatusCode(); to treat statusCode as error data response
				{
					//var responseContent = await response.Content.ReadAsStringAsync();
					//Console.WriteLine("UploadFileInModel_WithHttpClientAsync response :" + responseContent);
					TempData["success"] = "Add Brand successfully";
					return RedirectToAction("Index");
				}
				else
				{
					var errMsg = JsonConvert.DeserializeObject<dynamic>(response.Content.ReadAsStringAsync().Result);
					TempData["error"] = errMsg;
					return View(brand);
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
				return View(brand);
			}
		}

		public async Task<IActionResult> Delete(Guid Id)
		{
			//var response1 = await _shoppingStoreService.GetCategoryByIdAsync(Id);
			//TempData["success"] = response1;
			//return View();
			using HttpResponseMessage response = await _brandService.DeleteBrandAsync(Id);
			//response.EnsureSuccessStatusCode();
			if (response.IsSuccessStatusCode) // can use httpResponseMessage.EnsureSuccessStatusCode(); to treat statusCode as error data response
			{
				//var responseContent = await response.Content.ReadAsStringAsync();
				//Console.WriteLine("UploadFileInModel_WithHttpClientAsync response :" + responseContent);
				TempData["success"] = "Remove Brand successfully";
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
			ViewBag.Class = "/Admin/Brand";
			BrandDto brand = await _brandService.GetBrandByIdAsync(Id);
			//var jsonParent = JsonConvert.SerializeObject(brand);
			//BrandForEditDto brandEdit = JsonConvert.DeserializeObject<BrandForEditDto>(jsonParent);
			BrandForEditDto brandEdit = new BrandForEditDto()
			{
				Name = brand.Name,
				Description = brand.Description,
				Slug = brand.Slug,
				Status = brand.Status
			};
			return View(brandEdit);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(Guid Id, BrandForEditDto brand)
		{
            ViewBag.Class = "/Admin/Brand";
            if (ModelState.IsValid)
			{
				//add data to db
				//TempData["success"] = "Model da ok het";
				using HttpResponseMessage response = await _brandService.UpdateBrandAsync(Id, brand);
				//response.EnsureSuccessStatusCode();
				if (response.IsSuccessStatusCode) // can use httpResponseMessage.EnsureSuccessStatusCode(); to treat statusCode as error data response
				{
					//var responseContent = await response.Content.ReadAsStringAsync();
					//Console.WriteLine("UploadFileInModel_WithHttpClientAsync response :" + responseContent);
					TempData["success"] = "Update Brand successfully";
					return RedirectToAction("Index");
				}
				else
				{
					var errMsg = JsonConvert.DeserializeObject<dynamic>(response.Content.ReadAsStringAsync().Result);
					TempData["error"] = errMsg;
					return View(brand);
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
				return View(brand);
			}
		}
	}
}
