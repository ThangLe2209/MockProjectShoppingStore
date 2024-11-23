using Microsoft.Net.Http.Headers;
using ShoppingStore.Model;
using ShoppingStore.Model.Dtos;

namespace ShoppingStore.Client.Repository
{
	public class StatisticService
	{
		private readonly HttpClient _httpClient;
		private readonly IConfiguration Configuration;

		public StatisticService(HttpClient httpClient, IConfiguration configuration)
		{
			_httpClient = httpClient;
			Configuration = configuration;

			_httpClient.BaseAddress = new Uri($"{Configuration["ShoppingStoreAPIRoot"]}api/statistics/");
			_httpClient.DefaultRequestHeaders.Clear();
			_httpClient.DefaultRequestHeaders.Add(HeaderNames.Accept, "application/json");
		}

        // Statistics API
        public async Task<HttpResponseMessage> GetStatisticsAsync()
			=> await _httpClient.GetAsync($"");

        public async Task<HttpResponseMessage> GetStatisticsByStartEndDateAsync(DateTime startDate, DateTime endDate)
			=> await _httpClient.GetAsync($"getByStartEndDate?startDate={startDate}&endDate={endDate}");
    }
}
