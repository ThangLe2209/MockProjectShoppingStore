using ShoppingStore.Model;

namespace ShoppingStore.API.Services
{
	public interface IContactRepository
	{
		Task<IEnumerable<ContactModel>> GetContactsAsync();

		Task<bool> CheckContactNameAsync(string name);
		Task<bool> CheckContactNameUpdateAsync(Guid currentUpdatedId, string currentUpdatedContactName);
		Task<bool> CheckContactImageUrlAsync(string contactImageUrl);
		bool CheckContactImageUpload(IFormFile contactImageUpload);

		Task<string> AddContactImageAsync(IFormFile contactImageUpload);

		void DeleteOldContactImage(string oldImagePath);

		Task<ContactModel?> GetContactByIdAsync(Guid contactId);

		void AddContact(ContactModel contact);

		void DeleteContact(ContactModel contact);

		Task<bool> SaveChangesAsync();
    }
}
