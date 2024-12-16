using Microsoft.Net.Http.Headers;
using ShoppingStore.Model;
using ShoppingStore.Model.Dtos;

namespace ShoppingStore.Client.Repository
{
	public class SongService
    {
		private readonly HttpClient _httpClient;
		private readonly IConfiguration Configuration;

		public SongService(HttpClient httpClient, IConfiguration configuration)
		{
			_httpClient = httpClient;
			Configuration = configuration;

			_httpClient.BaseAddress = new Uri($"{Configuration["ShoppingStoreAPIRoot"]}api/songs/");
			_httpClient.DefaultRequestHeaders.Clear();
			_httpClient.DefaultRequestHeaders.Add(HeaderNames.Accept, "application/json");
		}

		// Sliders API
		public async Task<IEnumerable<SongDto>?> GetSongsAsync()
			=> await _httpClient.GetFromJsonAsync<IEnumerable<SongDto>>($"");

		public async Task<SongDto?> GetSongByIdAsync(Guid songId)
			=> await _httpClient.GetFromJsonAsync<SongDto>($"{songId}");
		public async Task<HttpResponseMessage> CreateSongAsync(dynamic songContent)
			=> await _httpClient.PostAsync($"", songContent);

		public async Task<HttpResponseMessage> UpdateSongAsync(Guid songId, dynamic songContent)
			=> await _httpClient.PutAsync($"{songId}", songContent);

		public async Task<HttpResponseMessage> DeleteSongAsync(Guid songId)
			=> await _httpClient.DeleteAsync($"{songId}");
	}
}
