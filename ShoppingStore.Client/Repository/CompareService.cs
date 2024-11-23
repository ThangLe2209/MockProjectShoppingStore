using Microsoft.Net.Http.Headers;
using ShoppingStore.Model;
using ShoppingStore.Model.Dtos;

namespace ShoppingStore.Client.Repository
{
	public class CompareService
	{
		private readonly HttpClient _httpClient;
		private readonly IConfiguration Configuration;

		public CompareService(HttpClient httpClient, IConfiguration configuration)
		{
			_httpClient = httpClient;
			Configuration = configuration;

			_httpClient.BaseAddress = new Uri($"{Configuration["ShoppingStoreAPIRoot"]}api/compares/");
			_httpClient.DefaultRequestHeaders.Clear();
			_httpClient.DefaultRequestHeaders.Add(HeaderNames.Accept, "application/json");
		}

		// Compare API

		public async Task<IEnumerable<CompareWithProductAndUserDto>?> GetCompareWithProductAndUser(Guid userSubId)
			=> await _httpClient.GetFromJsonAsync<IEnumerable<CompareWithProductAndUserDto>>($"getwithproductanduser/{userSubId}");
		public async Task<HttpResponseMessage> CreateCompareAsync(CompareForCreationDto compare)
			=> await _httpClient.PostAsJsonAsync($"", compare);

		public async Task<HttpResponseMessage> DeleteCompareAsync(Guid compareId)
			=> await _httpClient.DeleteAsync($"{compareId}");
	}
}
