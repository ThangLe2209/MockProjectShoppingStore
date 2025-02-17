using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ShoppingStore.Model.Dtos;
using ShoppingStore.Models;
using System.Security.Claims;

namespace ShoppingStore.Client.Repository.ViewComponents
{
    public class HeadersViewComponent : ViewComponent
    {
        private readonly WishlistService _wishlistService;
        private readonly CompareService _compareService;

        public HeadersViewComponent(WishlistService wishlistService, CompareService compareService)
        {
            _wishlistService = wishlistService;
            _compareService = compareService;
        }

        //public IViewComponentResult Invoke()
        //{
        //    List<CartItemModel> cartItems = HttpContext.Session.GetJson<List<CartItemModel>>("Cart") ?? new List<CartItemModel>();
        //    var totalCount = cartItems.Sum(x => x.Quantity);
        //    return View(totalCount);
        //}

        public async Task<IViewComponentResult> InvokeAsync()
        {
            List<CartItemModel> cartItems = HttpContext.Session.GetJson<List<CartItemModel>>("Cart") ?? new List<CartItemModel>();
            var totalCount = cartItems.Sum(x => x.Quantity);

            var wishListSessionData = HttpContext.Session.GetJson<List<WishlistWithProductAndUserDto>>("WishlistData");
            var compareSessionData = HttpContext.Session.GetJson<List<CompareWithProductAndUserDto>>("CompareData");
            var isUpdateData = HttpContext.Session.GetString("IsUpdateData") ?? "notUpdate";
            var wishListCount = wishListSessionData == null ? 0 : wishListSessionData.Count();
            var compareCount = compareSessionData == null ? 0 : compareSessionData.Count();
            //ViewBag.Test = compareSessionData == null ? 0 : compareSessionData.Count();
            //dynamic? a = new List<WishlistWithProductAndUserDto>() { };
            var user = User as ClaimsPrincipal;
            // https://stackoverflow.com/questions/19561877/is-it-possible-to-set-localstorage-or-session-variable-in-asp-net-page-and-read
            if (user.Identity.IsAuthenticated == false)
            {
                if (wishListSessionData != null) HttpContext.Session.Remove("WishlistData");
                if (compareSessionData != null) HttpContext.Session.Remove("CompareData");
            }
            else
            {
                var userSubId = user.Claims?.FirstOrDefault(x => x.Type == "sub")?.Value; // User as ClaimsPrincipal; => polymophism IClaimsPrincipal ClaimsPrincipal
                if (userSubId == null)
                {
                    TempData["error"] = "User don't have SubId Claim";
                    return View(totalCount);
                }
				//isUpdateData ko dung nha - luc dau co solution la check status a nhung sau do thi thay lai ko can ma ko nỡ xóa nha
				if (wishListSessionData == null || isUpdateData == "updateWishlist") // == null mean first time after login and the time after session expired but still login
                {
                    var wishListData = await _wishlistService.GetWishlistWithProductAndUser(new Guid(userSubId));
                    wishListCount = wishListData.Count(); // This line because SetJson below return null after first time login(this line only run to fix first time login)
                    HttpContext.Session.SetJson("WishlistData", wishListData); // first time null because that the website is not re-render eventhough already SetJson success => line above is fix for first time after login
					HttpContext.Session.SetString("IsUpdateData", "notUpdate");
                    //ViewBag.Test = JsonConvert.SerializeObject(wishListData);
                }
                if (compareSessionData == null || isUpdateData == "updateCompare")
                {
                    var compareData = await _compareService.GetCompareWithProductAndUser(new Guid(userSubId));
                    compareCount = compareData.Count(); // This line because SetJson below return null after first time login(this line only run to fix first time login)
                    HttpContext.Session.SetJson("CompareData", compareData);
                    HttpContext.Session.SetString("IsUpdateData", "notUpdate");
                }
            }
            //ViewBag.Test = JsonConvert.SerializeObject(wishListSessionData);
            //ViewBag.Test = 1000;
            return View(new { cartCount = totalCount, wishListCount = wishListCount, compareCount = compareCount });
        }
    }
}
