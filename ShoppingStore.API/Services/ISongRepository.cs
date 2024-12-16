using ShoppingStore.Model;

namespace ShoppingStore.API.Services
{
	public interface ISongRepository
	{
		Task<IEnumerable<SongModel>> GetSongsAsync();

        Task<SongModel?> GetSongByIdAsync(Guid songId);

		void AddSong(SongModel song);

        void DeleteSong(SongModel song);

        Task<bool> CheckSongNameAsync(string name);

        Task<bool> CheckSongImageUrlAsync(string songImageUrl);

        bool CheckSongImageUpload(IFormFile songImageUpload);

        Task<string> AddSongImageAsync(IFormFile songImageUpload);

        bool CheckSongUpload(IFormFile songUpload);

        Task<string> AddSongAsync(IFormFile songUpload);

        Task<bool> CheckSongNameUpdateAsync(Guid currentUpdatedId, string currentUpdatedSongName);
        void DeleteOldSongImage(string oldImagePath);

        void DeleteOldSong(string oldSongPath);

        Task<bool> SaveChangesAsync();

    }
}
