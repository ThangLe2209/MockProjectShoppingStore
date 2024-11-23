using Microsoft.Net.Http.Headers;
using ShoppingStore.Model;
using ShoppingStore.Model.Dtos;

namespace ShoppingStore.Client.Repository
{
	public class ContactService
	{
		private readonly HttpClient _httpClient;
		private readonly IConfiguration Configuration;

		public ContactService(HttpClient httpClient, IConfiguration configuration)
		{
			_httpClient = httpClient;
			Configuration = configuration;

			_httpClient.BaseAddress = new Uri($"{Configuration["ShoppingStoreAPIRoot"]}api/contacts/");
			_httpClient.DefaultRequestHeaders.Clear();
			_httpClient.DefaultRequestHeaders.Add(HeaderNames.Accept, "application/json");
		}

		// Sliders API
		public async Task<IEnumerable<ContactDto>?> GetContactsAsync()
			=> await _httpClient.GetFromJsonAsync<IEnumerable<ContactDto>>($"");

		public async Task<ContactDto?> GetContactByIdAsync(Guid contactId)
			=> await _httpClient.GetFromJsonAsync<ContactDto>($"{contactId}");
		public async Task<HttpResponseMessage> CreateContactAsync(dynamic contactContent)
			=> await _httpClient.PostAsync($"", contactContent);

		public async Task<HttpResponseMessage> UpdateContactAsync(Guid contactId, dynamic contactContent)
			=> await _httpClient.PutAsync($"{contactId}", contactContent);
		public async Task<HttpResponseMessage> DeleteContactAsync(Guid contactId)
			=> await _httpClient.DeleteAsync($"{contactId}");
	}
}
