using Microsoft.Net.Http.Headers;
using ShoppingStore.Model.Dtos;

namespace ShoppingStore.Client.Repository
{
    public class OrderDetailService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration Configuration;

        public OrderDetailService() { }
        public OrderDetailService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            Configuration = configuration;

            _httpClient.BaseAddress = new Uri($"{Configuration["ShoppingStoreAPIRoot"]}");
            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add(HeaderNames.Accept, "application/json");
        }

        //OrderDetail API
        public async Task<IEnumerable<OrderDetailDto>?> GetOrderDetailsByOrderCodeAsync(string orderCode) =>
            await _httpClient.GetFromJsonAsync<IEnumerable<OrderDetailDto>>($"/api/orders/orderDetails?orderCode={orderCode}");

        public virtual async Task<HttpResponseMessage> CreateOrderDetailsAsync(IEnumerable<OrderDetailForCreationDto> orderDetails)
            => await _httpClient.PostAsJsonAsync($"/api/orders/orderDetails", orderDetails);
    }
}
