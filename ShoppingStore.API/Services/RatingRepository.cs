using Microsoft.EntityFrameworkCore;
using ShoppingStore.API.DbContexts;
using ShoppingStore.Model;
using ShoppingStore.Model.Dtos;

namespace ShoppingStore.API.Services
{
    public class RatingRepository : IRatingRepository
    {
        private readonly ShoppingStoreContext _context;
        public RatingRepository(ShoppingStoreContext context)
        {
            _context = context ?? throw new ArgumentException(nameof(context));
        }

		public async Task<(IEnumerable<RatingModel>, PaginationMetadata)> GetRatingsByProductIdAsync(Guid productId, int pageNumber, int pageSize)
		{
			// collection to start from
			var collection = _context.Ratings as IQueryable<RatingModel>; // adding where clause for filtering and searching when needed 

			collection = collection.Where(r => r.ProductId == productId);
			var totalItemCount = await collection.CountAsync();

			var paginationMetadata = new PaginationMetadata(totalItemCount, pageSize, pageNumber);

			var collectionToReturn = await collection
							.Skip(pageSize * (pageNumber - 1)) // Paging (similar for filter and search) done at database level not after the records have been fetched thank to deferred execution => build all query first then call ToListAsync at final (line 56)
							.Take(pageSize)
							.ToListAsync();

			return (collectionToReturn, paginationMetadata);
		}

		public async Task<RatingModel> GetRatingByIdAsync(Guid Id)
        {
            return await _context.Ratings.FirstOrDefaultAsync(r => r.Id == Id);
        }

        public void AddRating(RatingModel rating)
        {
            _context.Ratings.Add(rating);
        }

        public async Task<bool> SaveChangesAsync()
        {
            return (await _context.SaveChangesAsync() >= 0);
        }
    }
}
