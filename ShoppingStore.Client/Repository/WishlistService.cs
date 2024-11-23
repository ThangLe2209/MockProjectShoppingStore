using Microsoft.Net.Http.Headers;
using ShoppingStore.Model;
using ShoppingStore.Model.Dtos;

namespace ShoppingStore.Client.Repository
{
	public class WishlistService
    {
		private readonly HttpClient _httpClient;
		private readonly IConfiguration Configuration;

		public WishlistService(HttpClient httpClient, IConfiguration configuration)
		{
			_httpClient = httpClient;
			Configuration = configuration;

			_httpClient.BaseAddress = new Uri($"{Configuration["ShoppingStoreAPIRoot"]}api/wishlists/");
			_httpClient.DefaultRequestHeaders.Clear();
			_httpClient.DefaultRequestHeaders.Add(HeaderNames.Accept, "application/json");
		}

		// Wishlist API

		public async Task<IEnumerable<WishlistWithProductAndUserDto>?> GetWishlistWithProductAndUser(Guid userSubId)
			=> await _httpClient.GetFromJsonAsync<IEnumerable<WishlistWithProductAndUserDto>>($"getwithproductanduser/{userSubId}");
		public async Task<HttpResponseMessage> CreateWishlistAsync(WishlistForCreationDto wishlist)
			=> await _httpClient.PostAsJsonAsync($"", wishlist);

		public async Task<HttpResponseMessage> DeleteWishlistAsync(Guid wishlistId)
			=> await _httpClient.DeleteAsync($"{wishlistId}");
	}
}
