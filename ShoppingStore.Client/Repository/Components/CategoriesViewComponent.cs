using Microsoft.AspNetCore.Mvc;
using ShoppingStore.Client.Controllers;
using ShoppingStore.Client.Repository;
using ShoppingStore.Model.Dtos;
namespace ShoppingStore.Client.Repository.ViewComponents
{
	public class CategoriesViewComponent: ViewComponent
	{
		private readonly CategoryService _categoryService;
		public CategoriesViewComponent(CategoryService categoryService)
		{
			_categoryService = categoryService;
		}

		public async Task<IViewComponentResult> InvokeAsync(int categoryMenuId = 1, string slugQueryString = "")
		{
            //var slugQueryString = Request.Query["slug"];
            //ViewBag.slugQueryString = slugQueryString.ToString().ToLower();
            ViewBag.slugQueryString = slugQueryString == "" ? Request.Query["slug"].ToString().ToLower() : slugQueryString;
            ViewBag.categoryMenuId = categoryMenuId;
            //         var categories = await _categoryService.GetCategoriesAsync();
            //return View(categories);
            using HttpResponseMessage httpResponseMessage = await _categoryService.GetCategoriesPaginateAsync("","", pageNumber: categoryMenuId);
            var callbackSuccess = (Stream contentStream, IEnumerable<CategoryDto> resultData) =>
            {
                var paginationDataJson = httpResponseMessage.Headers.GetValues("X-Pagination").FirstOrDefault();
                var paginationData = System.Text.Json.JsonSerializer.Deserialize<PaginationMetadata>(paginationDataJson);
                ViewBag.Pager = paginationData;
                return View(resultData);
            };
            var callbackFail = (string errMsg) =>
            {
                TempData["error"] = errMsg;
                ViewBag.Pager = new PaginationMetadata(0, 0, 1);
                return View(new List<CategoryDto>());
            };
            return await HandleResponseResult.HandleResult<IEnumerable<CategoryDto>>(httpResponseMessage, callbackSuccess, callbackFail);
        }
	}
}
