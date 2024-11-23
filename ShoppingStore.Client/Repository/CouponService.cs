using Microsoft.Net.Http.Headers;
using ShoppingStore.Model;
using ShoppingStore.Model.Dtos;

namespace ShoppingStore.Client.Repository
{
	public class CouponService
    {
		private readonly HttpClient _httpClient;
		private readonly IConfiguration Configuration;

		public CouponService() { } // unittest
        public CouponService(HttpClient httpClient, IConfiguration configuration)
		{
			_httpClient = httpClient;
			Configuration = configuration;

			_httpClient.BaseAddress = new Uri($"{Configuration["ShoppingStoreAPIRoot"]}api/coupons/");
			_httpClient.DefaultRequestHeaders.Clear();
			_httpClient.DefaultRequestHeaders.Add(HeaderNames.Accept, "application/json");
		}

        // Coupon API

        public async Task<IEnumerable<CouponDto>?> GetCouponsAsync() =>
            await _httpClient.GetFromJsonAsync<IEnumerable<CouponDto>>("");

        public async Task<CouponDto?> GetCouponByIdAsync(Guid couponId) =>
			await _httpClient.GetFromJsonAsync<CouponDto>($"{couponId}");

        public virtual async Task<decimal> GetNewGrandTotalByCoupon(decimal oldGrandTotal, string couponName) =>
			await _httpClient.GetFromJsonAsync<decimal>($"getNewGrandTotalByCoupon/{oldGrandTotal}/{couponName}");

        public virtual async Task<CouponDto?> GetCouponValidByNameAsync(string couponName) =>
			await _httpClient.GetFromJsonAsync<CouponDto>($"GetCouponValidByName?couponName={couponName}");

        public virtual async Task<HttpResponseMessage> GetCouponValidByNameWithErrorMessageAsync(string couponName) => // here is GetAsync return HttpResponseMessage
            await _httpClient.GetAsync($"GetCouponValidByName?couponName={couponName}");

        public virtual async Task<HttpResponseMessage> PartiallyUpdateCouponAsync(Guid couponId, dynamic patchDocument)
			=> await _httpClient.PatchAsync($"{couponId}", patchDocument);
        public async Task<HttpResponseMessage> CreateCouponAsync(CouponForCreationDto coupon)
			=> await _httpClient.PostAsJsonAsync($"", coupon);

        public async Task<HttpResponseMessage> UpdateCouponAsync(Guid couponId, CouponForEditDto coupon)
			=> await _httpClient.PutAsJsonAsync($"{couponId}", coupon);

        public async Task<HttpResponseMessage> DeleteCouponAsync(Guid couponId)
			=> await _httpClient.DeleteAsync($"{couponId}");
	}
}
