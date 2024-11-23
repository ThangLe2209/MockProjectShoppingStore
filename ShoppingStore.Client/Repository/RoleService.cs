using Microsoft.Net.Http.Headers;
using ShoppingStore.Model.Dtos;

namespace ShoppingStore.Client.Repository
{
    public class RoleService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration Configuration;

        public RoleService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            Configuration = configuration;

            _httpClient.BaseAddress = new Uri($"{Configuration["ShoppingStoreAPIRoot"]}");
            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add(HeaderNames.Accept, "application/json");
        }

        // Roles API
        public async Task<IEnumerable<RoleDto>?> GetRolesAsync() =>
            await _httpClient.GetFromJsonAsync<IEnumerable<RoleDto>>("/api/roles/");
        public async Task<RoleDto?> GetRoleByIdAsync(Guid roleId) =>
            await _httpClient.GetFromJsonAsync<RoleDto>($"/api/roles/{roleId}");
        public async Task<HttpResponseMessage> CreateRoleAsync(RoleForCreationDto role)
            => await _httpClient.PostAsJsonAsync($"/api/roles/", role);

        public async Task<HttpResponseMessage> UpdateRoleAsync(Guid roleId, RoleForEditDto role)
            => await _httpClient.PutAsJsonAsync($"/api/roles/{roleId}", role);

        public async Task<HttpResponseMessage> DeleteRoleAsync(Guid roleId)
            => await _httpClient.DeleteAsync($"/api/roles/{roleId}");
    }
}
