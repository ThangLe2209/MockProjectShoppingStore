using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using ShoppingStore.Model.Dtos;

namespace ShoppingStore.Client.Repository.Components
{
	public class BrandsViewComponent: ViewComponent
	{
		private readonly BrandService _brandService;

		public BrandsViewComponent(BrandService brandService)
		{
			_brandService = brandService;
		}

        //[OutputCache(Duration = 2400)]
		public async Task<IViewComponentResult> InvokeAsync(int brandMenuId, string slugQueryString = "")
		{
            //var slugQueryString = Request.Query["slug"];
			ViewBag.slugQueryString = slugQueryString == "" ? Request.Query["slug"].ToString().ToLower() : slugQueryString; // check Request from Action Navigate(not from url), not re-render page like react
			ViewBag.brandMenuId = brandMenuId;

            //return View(await _brandService.GetBrandsAsync());
            //dynamic? a1 = null;
            //using HttpResponseMessage httpResponseMessage1 = await _brandService.GetBrandsByOrderDetailsPaginate("", pageNumber: 1);
            //Func<Stream, IEnumerable<BrandDto>, IViewComponentResult?> callbackSuccess1 = (Stream contentStream, IEnumerable<BrandDto> resultData) =>
            //{
            //    var paginationDataJson = httpResponseMessage1.Headers.GetValues("X-Pagination").FirstOrDefault();
            //    var paginationData = System.Text.Json.JsonSerializer.Deserialize<PaginationMetadata>(paginationDataJson);
            //    a1 = resultData;
            //    return null; // Test dynamic function return only
            //};
            //Func<string, IViewComponentResult?> callbackFail1 = (string errMsg) =>
            //{
            //    TempData["error"] = errMsg;
            //    ViewBag.Pager = new PaginationMetadata(0, 0, 1);
            //    return null; // Test dynamic function return only
            //};
            //await HandleResponseResult.HandleResult<IEnumerable<BrandDto>, IViewComponentResult>(httpResponseMessage1, callbackSuccess1, callbackFail1); // Test dynamic function return only

            //var b = a1;
            //var a = 10;

            using HttpResponseMessage httpResponseMessage = await _brandService.GetBrandsByOrderDetailsPaginate("", pageNumber: brandMenuId);
            //using HttpResponseMessage httpResponseMessage = await _brandService.GetBrandsPaginateAsync("", pageNumber: brandMenuId);
            var callbackSuccess = (Stream contentStream, IEnumerable<BrandDto> resultData) =>
            {
                //IEnumerable<string>? paginationHeader;
                //var paginationDataJson = "{\"TotalItemCount\":10,\"TotalPageCount\":1,\"PageSize\":4,\"CurrentPage\":1}";
                //if (httpResponseMessage.Headers.TryGetValues("X-Pagination", out paginationHeader))
                //{
                //    paginationDataJson = paginationHeader.FirstOrDefault();
                //}

                var paginationDataJson = httpResponseMessage.Headers.GetValues("X-Pagination").FirstOrDefault();
                var paginationData = System.Text.Json.JsonSerializer.Deserialize<PaginationMetadata>(paginationDataJson);
                ViewBag.Pager = paginationData;
                return View(resultData);
            };
            var callbackFail = (string errMsg) =>
            {
                TempData["error"] = errMsg;
                ViewBag.Pager = new PaginationMetadata(0, 0, 1);
                return View(new List<BrandDto>());
            };
            return await HandleResponseResult.HandleResult<IEnumerable<BrandDto>>(httpResponseMessage, callbackSuccess, callbackFail);


            // Approach return Tuple in Backend Api Controller
            //var callbackSuccess = (Stream contentStream, (IEnumerable<BrandDto>, string) resultData) =>
            //{
            //    ViewBag.Pager = System.Text.Json.JsonSerializer.Deserialize<PaginationMetadata>(resultData.Item2);
            //    return View(resultData.Item1);
            //};
            //var callbackFail = (string errMsg) =>
            //{
            //    TempData["error"] = errMsg;
            //    ViewBag.Pager = new PaginationMetadata(0, 0, 1);
            //    return View(new List<BrandDto>());
            //};
            //return await HandleResponseResult.HandleResultTuple<(IEnumerable<BrandDto>, string)>(httpResponseMessage, callbackSuccess, callbackFail);
        }
	}
}
