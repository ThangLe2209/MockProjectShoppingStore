using Microsoft.Net.Http.Headers;
using ShoppingStore.Model.Dtos;

namespace ShoppingStore.Client.Repository
{
	public class CategoryService
	{
		private readonly HttpClient _httpClient;
		private readonly IConfiguration Configuration;

		public CategoryService() { }
        public CategoryService(HttpClient httpClient, IConfiguration configuration)
		{
			_httpClient = httpClient;
			Configuration = configuration;

			_httpClient.BaseAddress = new Uri($"{Configuration["ShoppingStoreAPIRoot"]}");
			_httpClient.DefaultRequestHeaders.Clear();
			_httpClient.DefaultRequestHeaders.Add(HeaderNames.Accept, "application/json");
		}

		// Categories API
		public virtual async Task<IEnumerable<CategoryDto>?> GetCategoriesAsync() =>
			await _httpClient.GetFromJsonAsync<IEnumerable<CategoryDto>>("/api/categories/");

        public async Task<HttpResponseMessage> GetCategoriesPaginateAsync(string? name = "", string? searchQuery = "", int pageNumber = 1, int pageSize = 4) =>
			await _httpClient.GetAsync($"/api/categories/paginate?name={name}&searchQuery={searchQuery}&pageNumber={pageNumber}&pageSize={pageSize}");

        public async Task<CategoryDto?> GetCategoryByIdAsync(Guid categoryId) =>
			 await _httpClient.GetFromJsonAsync<CategoryDto>($"/api/categories/{categoryId}");

		public async Task<HttpResponseMessage> CreateCategoryAsync(CategoryForCreationDto category)
			=> await _httpClient.PostAsJsonAsync($"/api/categories/", category);

		public async Task<HttpResponseMessage> UpdateCategoryAsync(Guid categoryId, CategoryForEditDto category)
			=> await _httpClient.PutAsJsonAsync($"/api/categories/{categoryId}", category);

		public async Task<HttpResponseMessage> DeleteCategoryAsync(Guid categoryId)
			=> await _httpClient.DeleteAsync($"/api/categories/{categoryId}");
	}
}
