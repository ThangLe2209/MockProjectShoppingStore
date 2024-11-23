using Microsoft.AspNetCore.Mvc;
using ShoppingStore.Model.Dtos;

namespace ShoppingStore.Client.Repository.Components
{
	public class RatingsViewComponent : ViewComponent
	{
		private readonly RatingService _ratingService;

		public RatingsViewComponent(RatingService ratingService)
		{
			_ratingService = ratingService;
		}

		public async Task<IViewComponentResult> InvokeAsync(Guid productId, int ratingPageId)
		{
			using HttpResponseMessage httpResponseMessage = await _ratingService.GetRatingsPaginateByProductIdAsync(productId, pageNumber: ratingPageId);
			var callbackSuccess = (Stream contentStream, IEnumerable<RatingDto> resultData) =>
			{
				var paginationDataJson = httpResponseMessage.Headers.GetValues("X-Pagination").FirstOrDefault();
				var paginationData = System.Text.Json.JsonSerializer.Deserialize<PaginationMetadata>(paginationDataJson);
				ViewBag.Pager = paginationData;
				ViewBag.RatingPageId = ratingPageId;
				ViewBag.ProductId = productId;
				return View(resultData);
			};
			var callbackFail = (string errMsg) =>
			{
				TempData["error"] = errMsg;
				ViewBag.Pager = new PaginationMetadata(0, 0, 1);
                ViewBag.RatingPageId = ratingPageId;
                ViewBag.ProductId = productId;
                return View(new List<RatingDto>());
			};
			return await HandleResponseResult.HandleResult<IEnumerable<RatingDto>>(httpResponseMessage, callbackSuccess, callbackFail);
		}
	}
}
