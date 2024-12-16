using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using ShoppingStore.API.DbContexts;
using ShoppingStore.Model;
using System.IO;

namespace ShoppingStore.API.Services
{
    public class SongRepository : ISongRepository
    {
        private readonly ShoppingStoreContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public SongRepository(ShoppingStoreContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context ?? throw new ArgumentException(nameof(context));
            _webHostEnvironment = webHostEnvironment ?? throw new ArgumentException(nameof(webHostEnvironment));
        }

        public void DeleteSong(SongModel song)
        {
            _context.Songs.Remove(song);
        }

        public void AddSong(SongModel song)
        {
            _context.Songs.Add(song);
        }

        public async Task<SongModel?> GetSongByIdAsync(Guid songId)
        {
            return await _context.Songs.FirstOrDefaultAsync(s => s.Id == songId);
        }

        public async Task<IEnumerable<SongModel>> GetSongsAsync()
        {
            //return await _context.Sliders.OrderByDescending(s => s.Id).ThenByDescending(s => s.UpdatedDate).ToListAsync();
            return await _context.Songs.OrderByDescending(s => s.UpdatedDate).ToListAsync();
        }

        public async Task<bool> CheckSongNameAsync(string name)
        {
            return await _context.Songs.AnyAsync(s => s.Name == name);
        }

        public async Task<bool> CheckSongImageUrlAsync(string songImageUrl)
        {
            return await _context.Songs.AnyAsync(s => s.Image == songImageUrl);
        }

        public bool CheckSongImageUpload(IFormFile songImageUpload)
        {
            // https://stackoverflow.com/questions/7470689/how-to-check-if-filename-contains-substring-in-c-sharp 
            string uploadsDir = Path.Combine(_webHostEnvironment.WebRootPath, "media\\songimages");
            bool isContainFileName = Directory.EnumerateFiles(uploadsDir).Any(f => f.Contains(songImageUpload.FileName));
            return isContainFileName;
        }

        public async Task<string> AddSongImageAsync(IFormFile songImageUpload)
        {
            string uploadsDir = Path.Combine(_webHostEnvironment.WebRootPath, "media/songimages");
            string imageName = Guid.NewGuid().ToString() + "_" + songImageUpload.FileName;
            string filePath = Path.Combine(uploadsDir, imageName);
            FileStream fs = new FileStream(filePath, FileMode.Create);
            await songImageUpload.CopyToAsync(fs); // copy image fo file by mode create
            fs.Close();
            return imageName;
        }

        public bool CheckSongUpload(IFormFile songUpload)
        {
            // https://stackoverflow.com/questions/7470689/how-to-check-if-filename-contains-substring-in-c-sharp 
            string uploadsDir = Path.Combine(_webHostEnvironment.WebRootPath, "media\\songs");
            bool isContainFileName = Directory.EnumerateFiles(uploadsDir).Any(f => f.Contains(songUpload.FileName));
            return isContainFileName;
        }

        public async Task<string> AddSongAsync(IFormFile songUpload)
        {
            string uploadsDir = Path.Combine(_webHostEnvironment.WebRootPath, "media/songs");
            string songName = Guid.NewGuid().ToString() + "_" + songUpload.FileName;
            string filePath = Path.Combine(uploadsDir, songName);
            FileStream fs = new FileStream(filePath, FileMode.Create);
            await songUpload.CopyToAsync(fs); // copy image fo file by mode create
            fs.Close();
            return songName;
        }

        public async Task<bool> CheckSongNameUpdateAsync(Guid currentUpdatedId, string currentUpdatedSongName)
        {
            return await _context.Sliders.AnyAsync(s => s.Id != currentUpdatedId && s.Name == currentUpdatedSongName);
        }

        public void DeleteOldSongImage(string oldImagePath)
        {
            try
            {
                string uploadsDir = Path.Combine(_webHostEnvironment.WebRootPath, "media/songimages");
                //delete old picture
                string oldfilePath = Path.Combine(uploadsDir, oldImagePath);

                if (System.IO.File.Exists(oldfilePath))
                {
                    System.IO.File.Delete(oldfilePath);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while deleting the song image.");
            }
        }

        public void DeleteOldSong(string oldSongPath)
        {
            try
            {
                string uploadsDir = Path.Combine(_webHostEnvironment.WebRootPath, "media/songs");
                //delete old picture
                string oldfilePath = Path.Combine(uploadsDir, oldSongPath);

                if (System.IO.File.Exists(oldfilePath))
                {
                    System.IO.File.Delete(oldfilePath);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while deleting the song.");
            }
        }

        public async Task<bool> SaveChangesAsync()
        {
            return (await _context.SaveChangesAsync() >= 0);
        }
    }
}
