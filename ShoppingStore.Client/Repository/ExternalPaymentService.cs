using Microsoft.Net.Http.Headers;
using ShoppingStore.Model;
using ShoppingStore.Model.Dtos;

namespace ShoppingStore.Client.Repository
{
	public class ExternalPaymentService
    {
		private readonly HttpClient _httpClient;
		private readonly IConfiguration Configuration;

		public ExternalPaymentService() { }
        public ExternalPaymentService(HttpClient httpClient, IConfiguration configuration)
		{
			_httpClient = httpClient;
			Configuration = configuration;

			_httpClient.BaseAddress = new Uri($"{Configuration["ShoppingStoreAPIRoot"]}api/externalpayments/");
			_httpClient.DefaultRequestHeaders.Clear();
			_httpClient.DefaultRequestHeaders.Add(HeaderNames.Accept, "application/json");
		}

        // External Payments API

        public async Task<HttpResponseMessage> CreateVnpayAsync(VnpayForCreationDto vnpayItem)
			=> await _httpClient.PostAsJsonAsync($"addVnpay", vnpayItem);

        public async Task<VnpayDto?> GetVnPayByPaymentIdAsync(string paymentId) =>
			await _httpClient.GetFromJsonAsync<VnpayDto>($"getVnpayByPaymentId/{paymentId}");

		public async Task<HttpResponseMessage> CheckValidPaymentPrice(decimal rawTotal, string? couponName, string? shippingPrice, string paymentAmount)
			=> await _httpClient.GetAsync($"checkValidPaymentPrice?rawTotal={rawTotal}&couponName={couponName}&shippingPrice={shippingPrice}&paymentAmount={paymentAmount}");
    }
}
