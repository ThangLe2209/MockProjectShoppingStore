using Microsoft.Net.Http.Headers;
using ShoppingStore.Model;
using ShoppingStore.Model.Dtos;

namespace ShoppingStore.Client.Repository
{
	public class StatisticProductOrderService
    {
		private readonly HttpClient _httpClient;
		private readonly IConfiguration Configuration;

		public StatisticProductOrderService() { }
        public StatisticProductOrderService(HttpClient httpClient, IConfiguration configuration)
		{
			_httpClient = httpClient;
			Configuration = configuration;

			_httpClient.BaseAddress = new Uri($"{Configuration["ShoppingStoreAPIRoot"]}api/statisticProductOrders/");
			_httpClient.DefaultRequestHeaders.Clear();
			_httpClient.DefaultRequestHeaders.Add(HeaderNames.Accept, "application/json");
		}

        // Statistics API
        public virtual async Task<HttpResponseMessage> CreateStatisticalProductOrdersAsync(IEnumerable<StatisticalProductOrderForCreationDto> statisticalProductOrders)
			=> await _httpClient.PostAsJsonAsync($"", statisticalProductOrders);


    }
}
