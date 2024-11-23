using ShoppingStore.Model;

namespace ShoppingStore.API.Services
{
    public interface IProductRepository
    {
        Task<IEnumerable<ProductModel>> GetProductsAsync();
        Task<IEnumerable<ProductModel>> GetProductsAsync(string? searchTerm);
        Task<(IEnumerable<ProductModel>, PaginationMetadata)> GetProductsAsync(string? searchTerm,string? sortBy,int?min, int? max, int pageNumber, int pageSize);

        Task<(IEnumerable<ProductModel>, PaginationMetadata)> GetProductsByBrandSlugAsync(string? slug, string? sortBy, int? min, int? max, int pageNumber, int pageSize);
        Task<(IEnumerable<ProductModel>, PaginationMetadata)> GetProductsByCategorySlugAsync(string? slug, string? sortBy, int? min, int? max, int pageNumber, int pageSize);

        Task<ProductModel?> GetProductAsync(Guid productId);
        Task<ProductModel?> GetProductBySlugAsync(string? slug);

        Task AddProductAsync(ProductModel product);

        void DeleteProduct(ProductModel product);
        void DeleteProductImage(string productImagePath);
        Task<bool> SaveChangesAsync();
    }
}
