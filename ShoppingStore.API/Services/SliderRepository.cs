using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using ShoppingStore.API.DbContexts;
using ShoppingStore.Model;
using System.IO;

namespace ShoppingStore.API.Services
{
    public class SliderRepository : ISliderRepository
    {
        private readonly ShoppingStoreContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public SliderRepository(ShoppingStoreContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context ?? throw new ArgumentException(nameof(context));
            _webHostEnvironment = webHostEnvironment ?? throw new ArgumentException(nameof(webHostEnvironment));
        }

        public void DeleteSlider(SliderModel slider)
        {
            _context.Sliders.Remove(slider);
        }

        public void AddSlider(SliderModel slider)
        {
            _context.Sliders.Add(slider);
        }

        public async Task<string> AddSliderImageAsync(IFormFile silderImageUpload)
        {
            string uploadsDir = Path.Combine(_webHostEnvironment.WebRootPath, "media/sliders");
            string imageName = Guid.NewGuid().ToString() + "_" + silderImageUpload.FileName;
            string filePath = Path.Combine(uploadsDir, imageName);
            FileStream fs = new FileStream(filePath, FileMode.Create);
            await silderImageUpload.CopyToAsync(fs); // copy image fo file by mode create
            fs.Close();
            return imageName;
        }

        public void DeleteOldSliderImage(string oldImagePath)
        {
            try
            {
                string uploadsDir = Path.Combine(_webHostEnvironment.WebRootPath, "media/sliders");
                //delete old picture
                string oldfilePath = Path.Combine(uploadsDir, oldImagePath);

                if (System.IO.File.Exists(oldfilePath))
                {
                    System.IO.File.Delete(oldfilePath);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while deleting the slider image.");
            }
        }

        public bool CheckSliderImageUpload(IFormFile silderImageUpload)
        {
            // https://stackoverflow.com/questions/7470689/how-to-check-if-filename-contains-substring-in-c-sharp 
            string uploadsDir = Path.Combine(_webHostEnvironment.WebRootPath, "media\\sliders");
            bool isContainFileName = Directory.EnumerateFiles(uploadsDir).Any(f => f.Contains(silderImageUpload.FileName));
            return isContainFileName;
        }

        public async Task<bool> CheckSliderImageUrlAsync(string silderImageUrl)
        {
            return await _context.Sliders.AnyAsync(s => s.Image == silderImageUrl);
        }

        public async Task<bool> CheckSliderNameAsync(string name)
        {
            return await _context.Sliders.AnyAsync(s => s.Name == name);
        }

        public async Task<bool> CheckSliderNameUpdateAsync(Guid currentUpdatedId, string currentUpdatedSliderName)
        {
            return await _context.Sliders.AnyAsync(s => s.Id != currentUpdatedId && s.Name == currentUpdatedSliderName);
        }

        public async Task<SliderModel?> GetSliderByIdAsync(Guid sliderId)
        {
            return await _context.Sliders.FirstOrDefaultAsync(s => s.Id == sliderId);
        }

        public async Task<IEnumerable<SliderModel>> GetSlidersAsync()
        {
            //return await _context.Sliders.OrderByDescending(s => s.Id).ThenByDescending(s => s.UpdatedDate).ToListAsync();
            return await _context.Sliders.OrderByDescending(s => s.UpdatedDate).ToListAsync();
        }

        public async Task<bool> SaveChangesAsync()
        {
            return (await _context.SaveChangesAsync() >= 0);
        }
    }
}
