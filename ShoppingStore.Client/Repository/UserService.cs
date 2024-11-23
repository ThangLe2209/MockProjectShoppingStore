using Microsoft.Net.Http.Headers;
using ShoppingStore.Model.Dtos;

namespace ShoppingStore.Client.Repository
{
    public class UserService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration Configuration;

        public UserService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            Configuration = configuration;

            _httpClient.BaseAddress = new Uri($"{Configuration["ShoppingStoreAPIRoot"]}");
            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add(HeaderNames.Accept, "application/json");
        }

        // Users API
        public async Task<IEnumerable<UserDto>?> GetUsersAsync() =>
            await _httpClient.GetFromJsonAsync<IEnumerable<UserDto>>("/api/users/");

        public async Task<UserDto?> GetUserByIdAsync(Guid userId) =>
            await _httpClient.GetFromJsonAsync<UserDto>($"/api/users/{userId}");
        public async Task<UserDto?> GetUserByIdAsync(Guid userId, string? claimType) =>
            await _httpClient.GetFromJsonAsync<UserDto>($"/api/users/{userId}?type={claimType}");

        public async Task<UserDto?> GetUserByEmailAsync(string? userEmail) =>
            await _httpClient.GetFromJsonAsync<UserDto>($"/api/users/getUserByEmail?userEmail={userEmail}");

        public async Task<HttpResponseMessage> CreateUserAsync(UserForCreationDto user)
            => await _httpClient.PostAsJsonAsync($"/api/users/", user);

        public async Task<HttpResponseMessage> ActiveUserAsync(string statusCode)
             => await _httpClient.PostAsJsonAsync($"/api/users/activeAccount?statusCode={statusCode}", statusCode);

        public async Task<HttpResponseMessage> UpdateUserAsync(Guid userId, UserForEditDto user)
             => await _httpClient.PutAsJsonAsync($"/api/users/{userId}", user);

        public async Task<HttpResponseMessage> DeleteUserAsync(Guid userId)
            => await _httpClient.DeleteAsync($"/api/users/{userId}");
    }
}
