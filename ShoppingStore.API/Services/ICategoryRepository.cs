using ShoppingStore.Model;

namespace ShoppingStore.API.Services
{
    public interface ICategoryRepository
    {
        Task<IEnumerable<CategoryModel>> GetCategoriesAsync();

        Task<(IEnumerable<CategoryModel>, PaginationMetadata)> GetCategoriesAsync(string? name, string? searchQuery, int pageNumber, int pageSize);

        void AddCategory(CategoryModel category);

        void DeleteCategory(CategoryModel category);

        Task<CategoryModel> GetCategoryById(Guid categoryId);
        Task<CategoryModel> GetCategoryBySlug(string? slug);

        Task<bool> SaveChangesAsync();
    }
}
