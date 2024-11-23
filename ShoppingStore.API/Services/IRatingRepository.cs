using ShoppingStore.Model;
using ShoppingStore.Model.Entities;

namespace ShoppingStore.API.Services
{
    public interface IRatingRepository
	{
		Task<(IEnumerable<RatingModel>, PaginationMetadata)> GetRatingsByProductIdAsync(Guid productId, int pageNumber, int pageSize);
		Task<RatingModel> GetRatingByIdAsync(Guid Id);

		void AddRating(RatingModel rating);

        Task<bool> SaveChangesAsync();
    }
}