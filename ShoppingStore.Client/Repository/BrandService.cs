using Microsoft.Net.Http.Headers;
using ShoppingStore.Model.Dtos;

namespace ShoppingStore.Client.Repository
{
	public class BrandService
	{
		private readonly HttpClient _httpClient;
		private readonly IConfiguration Configuration;

        public BrandService() { }
        public BrandService(HttpClient httpClient, IConfiguration configuration)
		{
			_httpClient = httpClient;
			Configuration = configuration;

			_httpClient.BaseAddress = new Uri($"{Configuration["ShoppingStoreAPIRoot"]}");
			_httpClient.DefaultRequestHeaders.Clear();
			_httpClient.DefaultRequestHeaders.Add(HeaderNames.Accept, "application/json");
		}

		// Brands API
		public virtual async Task<IEnumerable<BrandDto>?> GetBrandsAsync() =>
			await _httpClient.GetFromJsonAsync<IEnumerable<BrandDto>>("/api/brands/");

        public async Task<HttpResponseMessage> GetBrandsPaginateAsync(string? searchTerm, int pageNumber = 1, int pageSize = 2) =>
			await _httpClient.GetAsync($"/api/brands/paginate?searchTerm={searchTerm}&pageNumber={pageNumber}&pageSize={pageSize}");

        public virtual async Task<HttpResponseMessage> GetBrandsByOrderDetailsPaginate(string? searchTerm = "", int pageNumber = 1, int pageSize = 4) =>
			await _httpClient.GetAsync($"/api/brands/getbrandsByOrderDetailsPaginate?searchTerm={searchTerm}&pageNumber={pageNumber}&pageSize={pageSize}");

        public async Task<BrandDto?> GetBrandByIdAsync(Guid brandId) =>
			await _httpClient.GetFromJsonAsync<BrandDto>($"/api/brands/{brandId}");

		public async Task<HttpResponseMessage> CreateBrandAsync(BrandForCreationDto brand)
			=> await _httpClient.PostAsJsonAsync($"/api/brands/", brand);

		public async Task<HttpResponseMessage> UpdateBrandAsync(Guid brandId, BrandForEditDto brand)
			=> await _httpClient.PutAsJsonAsync($"/api/brands/{brandId}", brand);

		public async Task<HttpResponseMessage> DeleteBrandAsync(Guid brandId)
			=> await _httpClient.DeleteAsync($"/api/brands/{brandId}");
	}
}
