using Microsoft.Net.Http.Headers;
using ShoppingStore.Model.Dtos;

namespace ShoppingStore.Client.Repository
{
    public class RatingService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration Configuration;

        public RatingService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            Configuration = configuration;

            _httpClient.BaseAddress = new Uri($"{Configuration["ShoppingStoreAPIRoot"]}");
            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add(HeaderNames.Accept, "application/json");
        }

        //Ratings API
        public async Task<HttpResponseMessage> CreateRatingAsync(RatingForCreationDto rating)
            => await _httpClient.PostAsJsonAsync($"/api/ratings/", rating);

		public async Task<HttpResponseMessage> GetRatingsPaginateByProductIdAsync(Guid productId, int pageNumber = 1, int pageSize = 2)
	        => await _httpClient.GetAsync($"/api/ratings/paginate/{productId}?pageNumber={pageNumber}&pageSize={pageSize}");
	}
}
