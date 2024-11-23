using Microsoft.Net.Http.Headers;
using ShoppingStore.Model;
using ShoppingStore.Model.Dtos;
using System.Net.Http;

namespace ShoppingStore.Client.Repository
{
	public class ProductQuantityService
	{
		private readonly HttpClient _httpClient;
		private readonly IConfiguration Configuration;

		public ProductQuantityService() {}
        public ProductQuantityService(HttpClient httpClient, IConfiguration configuration)
		{
			_httpClient = httpClient;
			Configuration = configuration;

			_httpClient.BaseAddress = new Uri($"{Configuration["ShoppingStoreAPIRoot"]}api/productQuantities/");
			_httpClient.DefaultRequestHeaders.Clear();
			_httpClient.DefaultRequestHeaders.Add(HeaderNames.Accept, "application/json");
		}

        // Product Quantity API
        public async Task<HttpResponseMessage> AddProductQuantityAsync(ProductQuantityForCreationDto productQuantity)
			=> await _httpClient.PostAsJsonAsync($"", productQuantity); // context use in backend Repository will be _context.ProductQuantities (just lazy to create seperate here)
        public async Task<IEnumerable<ProductQuantityDto>?> GetProductQuatitiesByProductId(Guid productId)
			=> await _httpClient.GetFromJsonAsync<IEnumerable<ProductQuantityDto>>($"getWithProductId/{productId}");
    }
}
