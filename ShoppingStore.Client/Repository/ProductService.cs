using Microsoft.AspNetCore.JsonPatch;
using Microsoft.Net.Http.Headers;
using ShoppingStore.Model.Dtos;
using System.Net.Http;

namespace ShoppingStore.Client.Repository
{
	public class ProductService
	{
		private readonly HttpClient _httpClient;
		private readonly IConfiguration Configuration;

		public ProductService() { } // unittest
        public ProductService(HttpClient httpClient, IConfiguration configuration)
		{
			_httpClient = httpClient;
			Configuration = configuration;

			_httpClient.BaseAddress = new Uri($"{Configuration["ShoppingStoreAPIRoot"]}");
			_httpClient.DefaultRequestHeaders.Clear();
			_httpClient.DefaultRequestHeaders.Add(HeaderNames.Accept, "application/json");
		}

		// Products API
		public async Task<IEnumerable<ProductDto>?> GetProductsAsync(string? searchTerm) =>
			await _httpClient.GetFromJsonAsync<IEnumerable<ProductDto>>($"/api/products?searchTerm={searchTerm}");
        public async Task<HttpResponseMessage> GetProductsPaginateAsync(string? searchTerm, string sortBy,int? min,int? max, int pageNumber = 1, int pageSize = 6) =>
			await _httpClient.GetAsync($"/api/products/paginate?searchTerm={searchTerm}&sortBy={sortBy}&min={min}&max={max}&pageNumber={pageNumber}&pageSize={pageSize}");
        public async Task<HttpResponseMessage> GetProductsByBrandSlug(string? slug, string sortBy, int? min, int? max, int pageNumber = 1, int pageSize = 6) =>
			 await _httpClient.GetAsync("/api/products/productByBrandSlug?slug=" + slug + $"&sortBy={sortBy}&min={min}&max={max}&pageNumber={pageNumber}&pageSize={pageSize}");
		public async Task<HttpResponseMessage> GetProductsByCategorySlugAsync(string? slug, string sortBy, int? min, int? max, int pageNumber = 1, int pageSize = 6) =>
			 await _httpClient.GetAsync("/api/products/productByCategorySlug?slug=" + slug + $"&sortBy={sortBy}&min={min}&max={max}&pageNumber={pageNumber}&pageSize={pageSize}");
		public async Task<ProductDto?> GetProductByProductSlugAsync(string? slug) =>
			 await _httpClient.GetFromJsonAsync<ProductDto>($"/api/products/productSlug?slug={slug}");
		public virtual async Task<ProductDto?> GetProductByIdAsync(Guid productId) =>
			 await _httpClient.GetFromJsonAsync<ProductDto>($"/api/products/{productId}");

		public virtual async Task<HttpResponseMessage> CreateProductAsync(dynamic productContent)
			=> await _httpClient.PostAsync($"/api/products/", productContent);

        public async Task<HttpResponseMessage> UpdateProductAsync(Guid productId, dynamic productContent)
			=> await _httpClient.PutAsync($"/api/products/{productId}", productContent);

		public async Task<HttpResponseMessage> DeleteProductAsync(Guid productId)
			=> await _httpClient.DeleteAsync($"/api/products/{productId}");

        public async Task<HttpResponseMessage> PartiallyUpdateProductAsync(Guid productId, dynamic patchDocument)
			=> await _httpClient.PatchAsync($"/api/products/{productId}", patchDocument);

        public virtual async Task<HttpResponseMessage> PartiallyUpdateProductsAsync(IDictionary<Guid, string> patchDocument)
    => await _httpClient.PatchAsJsonAsync($"/api/products/partiallyUpdateProducts", patchDocument);
    }
}
