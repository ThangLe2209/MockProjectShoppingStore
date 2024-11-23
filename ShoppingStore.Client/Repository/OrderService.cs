using Microsoft.Net.Http.Headers;
using ShoppingStore.Model.Dtos;

namespace ShoppingStore.Client.Repository
{
    public class OrderService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration Configuration;

        public OrderService() { }
        public OrderService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            Configuration = configuration;

            _httpClient.BaseAddress = new Uri($"{Configuration["ShoppingStoreAPIRoot"]}");
            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add(HeaderNames.Accept, "application/json");
        }

        //Orders API
        public async Task<IEnumerable<OrderDto>?> GetOrdersAsync() =>
            await _httpClient.GetFromJsonAsync<IEnumerable<OrderDto>>("/api/orders/");

        public async Task<IEnumerable<OrderDto>?> GetOrdersByUserEmailAsync(string userEmail) =>
            await _httpClient.GetFromJsonAsync<IEnumerable<OrderDto>>($"/api/orders/getByUserEmail/{userEmail}");

        public async Task<OrderDto?> GetOrderByOrderCodeAsync(string orderCode) =>
            await _httpClient.GetFromJsonAsync<OrderDto>($"/api/orders/orderByOrderCode?orderCode={orderCode}");
        public virtual async Task<HttpResponseMessage> CreateOrderAsync(OrderForCreationDto order)
            => await _httpClient.PostAsJsonAsync($"/api/orders/", order);

        public async Task<HttpResponseMessage> UpdateOrderAsync(string orderCode, int status)
            => await _httpClient.PutAsJsonAsync($"/api/orders", new { orderCode = orderCode, status = status });

		public async Task<HttpResponseMessage> DeleteOrderAsync(Guid orderId)
            => await _httpClient.DeleteAsync($"/api/orders/{orderId}");
    }
}
