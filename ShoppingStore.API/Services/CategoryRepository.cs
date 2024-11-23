using Microsoft.EntityFrameworkCore;
using ShoppingStore.API.DbContexts;
using ShoppingStore.Model;

namespace ShoppingStore.API.Services
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly ShoppingStoreContext _context;
        public CategoryRepository(ShoppingStoreContext context)
        {
            _context = context ?? throw new ArgumentException(nameof(context));
        }

        public void AddCategory(CategoryModel category)
        {
            _context.Categories.Add(category);
        }

        public void DeleteCategory(CategoryModel category)
        {
            _context?.Categories.Remove(category);
        }

        public async Task<IEnumerable<CategoryModel>> GetCategoriesAsync()
        {
            return await _context.Categories.ToListAsync();
        }

		public async Task<(IEnumerable<CategoryModel>, PaginationMetadata)> GetCategoriesAsync(string? name, string? searchQuery, int pageNumber, int pageSize)

		{

			// collection to start from
			var collection = _context.Categories as IQueryable<CategoryModel>; // adding where clause for filtering and searching when needed 

			if (!string.IsNullOrWhiteSpace(name))
			{
				name = name.Trim();
				collection = collection.Where(c => c.Name == name);
			}

			if (!string.IsNullOrWhiteSpace(searchQuery))
			{
				searchQuery = searchQuery.Trim();
				collection = collection.Where(a => a.Name.Contains(searchQuery)
						|| (a.Description != null && a.Description.Contains(searchQuery)));
			}

			var totalItemCount = await collection.CountAsync();

			var paginationMetadata = new PaginationMetadata(totalItemCount, pageSize, pageNumber);

			var collectionToReturn = await collection
							.OrderBy(c => c.Name)
							.Skip(pageSize * (pageNumber - 1)) // Paging (similar for filter and search) done at database level not after the records have been fetched thank to deferred execution => build all query first then call ToListAsync at final (line 56)
							.Take(pageSize)
							.ToListAsync();

			return (collectionToReturn, paginationMetadata);
		}

		public async Task<CategoryModel> GetCategoryById(Guid categoryId)
        {
			return await _context.Categories.FirstOrDefaultAsync(c => c.Id == categoryId);
		}

        public async Task<CategoryModel> GetCategoryBySlug(string? slug)
        {
			return await _context.Categories.FirstOrDefaultAsync(c => c.Slug == slug);
		}

        public async Task<bool> SaveChangesAsync()
        {
            return (await _context.SaveChangesAsync() >= 0);
        }
    }
}
