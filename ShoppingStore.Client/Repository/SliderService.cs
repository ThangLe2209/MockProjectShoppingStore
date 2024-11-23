using Microsoft.Net.Http.Headers;
using ShoppingStore.Model;
using ShoppingStore.Model.Dtos;

namespace ShoppingStore.Client.Repository
{
	public class SliderService
	{
		private readonly HttpClient _httpClient;
		private readonly IConfiguration Configuration;

		public SliderService(HttpClient httpClient, IConfiguration configuration)
		{
			_httpClient = httpClient;
			Configuration = configuration;

			_httpClient.BaseAddress = new Uri($"{Configuration["ShoppingStoreAPIRoot"]}api/sliders/");
			_httpClient.DefaultRequestHeaders.Clear();
			_httpClient.DefaultRequestHeaders.Add(HeaderNames.Accept, "application/json");
		}

		// Sliders API
		public async Task<IEnumerable<SliderDto>?> GetSlidersAsync()
			=> await _httpClient.GetFromJsonAsync<IEnumerable<SliderDto>>($"");

        public async Task<SliderDto?> GetSliderByIdAsync(Guid sliderId)
            => await _httpClient.GetFromJsonAsync<SliderDto>($"{sliderId}");
        public async Task<HttpResponseMessage> CreateSliderAsync(dynamic sliderContent)
			=> await _httpClient.PostAsync($"", sliderContent);

        public async Task<HttpResponseMessage> UpdateSliderAsync(Guid sliderId, dynamic sliderContent)
			=> await _httpClient.PutAsync($"{sliderId}", sliderContent);

        public async Task<HttpResponseMessage> DeleteSliderAsync(Guid sliderId)
			=> await _httpClient.DeleteAsync($"{sliderId}");
    }
}
