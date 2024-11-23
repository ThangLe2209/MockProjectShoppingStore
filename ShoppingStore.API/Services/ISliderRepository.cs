using ShoppingStore.Model;

namespace ShoppingStore.API.Services
{
	public interface ISliderRepository
	{
		Task<IEnumerable<SliderModel>> GetSlidersAsync();

		Task<bool> CheckSliderNameAsync(string name);
		Task<bool> CheckSliderNameUpdateAsync(Guid currentUpdatedId, string currentUpdatedSliderName);
		Task<bool> CheckSliderImageUrlAsync(string silderImageUrl);
        bool CheckSliderImageUpload(IFormFile silderImageUpload);

		Task<string> AddSliderImageAsync(IFormFile silderImageUpload);

		void DeleteOldSliderImage(string oldImagePath);

        Task<SliderModel?> GetSliderByIdAsync(Guid sliderId);

        void AddSlider(SliderModel slider);

        void DeleteSlider(SliderModel slider);

        Task<bool> SaveChangesAsync();
    }
}
