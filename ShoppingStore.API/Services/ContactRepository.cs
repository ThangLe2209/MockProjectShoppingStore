using Microsoft.EntityFrameworkCore;
using ShoppingStore.API.DbContexts;
using ShoppingStore.Model;

namespace ShoppingStore.API.Services
{
    public class ContactRepository : IContactRepository
    {
        private readonly ShoppingStoreContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public ContactRepository(ShoppingStoreContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context ?? throw new ArgumentException(nameof(context));
            _webHostEnvironment = webHostEnvironment ?? throw new ArgumentException(nameof(webHostEnvironment));
        }

        public void AddContact(ContactModel contact)
        {
            _context.Contacts.Add(contact);
        }

        public async Task<string> AddContactImageAsync(IFormFile contactImageUpload)
        {
            string uploadsDir = Path.Combine(_webHostEnvironment.WebRootPath, "media/logo");
            string imageName = Guid.NewGuid().ToString() + "_" + contactImageUpload.FileName;
            string filePath = Path.Combine(uploadsDir, imageName);
            FileStream fs = new FileStream(filePath, FileMode.Create);
            await contactImageUpload.CopyToAsync(fs); // copy image fo file by mode create
            fs.Close();
            return imageName;
        }

        public bool CheckContactImageUpload(IFormFile contactImageUpload)
        {
            // https://stackoverflow.com/questions/7470689/how-to-check-if-filename-contains-substring-in-c-sharp 
            string uploadsDir = Path.Combine(_webHostEnvironment.WebRootPath, "media\\logo");
            bool isContainFileName = Directory.EnumerateFiles(uploadsDir).Any(f => f.Contains(contactImageUpload.FileName));
            return isContainFileName;
        }

        public async Task<bool> CheckContactImageUrlAsync(string contactImageUrl)
        {
            return await _context.Contacts.AnyAsync(s => s.LogoImg == contactImageUrl);
        }

        public async Task<bool> CheckContactNameAsync(string name)
        {
            return await _context.Contacts.AnyAsync(s => s.Name == name);
        }

        public async Task<bool> CheckContactNameUpdateAsync(Guid currentUpdatedId, string currentUpdatedContactName)
        {
            return await _context.Contacts.AnyAsync(s => s.Id != currentUpdatedId && s.Name == currentUpdatedContactName);
        }

        public void DeleteContact(ContactModel contact)
        {
            _context.Contacts.Remove(contact);
        }

        public void DeleteOldContactImage(string oldImagePath)
        {
            try
            {
                string uploadsDir = Path.Combine(_webHostEnvironment.WebRootPath, "media/logo");
                //delete old picture
                string oldfilePath = Path.Combine(uploadsDir, oldImagePath);

                if (System.IO.File.Exists(oldfilePath))
                {
                    System.IO.File.Delete(oldfilePath);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while deleting the contact logo image.");
            }
        }

        public async Task<ContactModel?> GetContactByIdAsync(Guid contactId)
        {
            return await _context.Contacts.FirstOrDefaultAsync(c => c.Id == contactId);
        }

        public async Task<IEnumerable<ContactModel>> GetContactsAsync()
        {
            return await _context.Contacts.OrderByDescending(s => s.UpdatedDate).ToListAsync();
        }

        public async Task<bool> SaveChangesAsync()
        {
            return (await _context.SaveChangesAsync() >= 0);
        }
    }
}
