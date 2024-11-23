using Microsoft.Net.Http.Headers;
using ShoppingStore.Model;
using ShoppingStore.Model.Dtos;
using System.Net.Http.Json;

namespace ShoppingStore.Client.Repository
{
	public class ShippingService
    {
		private readonly HttpClient _httpClient;
		private readonly IConfiguration Configuration;
		private readonly IHttpContextAccessor _httpContextAccessor;

		public ShippingService() { } // unittest

		public ShippingService(HttpClient httpClient, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
		{
			_httpClient = httpClient;
			Configuration = configuration;
			_httpContextAccessor = httpContextAccessor;

			_httpClient.BaseAddress = new Uri($"{Configuration["ShoppingStoreAPIRoot"]}api/shippings/");
			_httpClient.DefaultRequestHeaders.Clear();
			_httpClient.DefaultRequestHeaders.Add(HeaderNames.Accept, "application/json");
			
			// Add cookies information to apis
			//var cookiesCollection = _httpContextAccessor.HttpContext.Request.Cookies;
			//foreach (var (key, value) in cookiesCollection)
			//{
   //             _httpClient.DefaultRequestHeaders.Add("Cookie", $"{key}={value}");
   //         }
        }

		// Shippings API
		public async Task<IEnumerable<ShippingDto>?> GetShippingsAsync()
			=> await _httpClient.GetFromJsonAsync<IEnumerable<ShippingDto>>($"");

		public virtual async Task<ShippingDto?> GetShippingByGeoDataAsync(string City, string District, string Ward)
			=> await _httpClient.GetFromJsonAsync<ShippingDto>($"getByGeoData?City={City}&District={District}&Ward={Ward}");
		public async Task<HttpResponseMessage> CreateShippingAsync(ShippingForCreationDto shipping)
			=> await _httpClient.PostAsJsonAsync($"", shipping);

		//     public async Task<HttpResponseMessage> UpdateSliderAsync(Guid sliderId, dynamic sliderContent)
		//=> await _httpClient.PutAsync($"{sliderId}", sliderContent);

		public async Task<HttpResponseMessage> DeleteShippingAsync(Guid shippingId)
			=> await _httpClient.DeleteAsync($"{shippingId}");
	}
}
