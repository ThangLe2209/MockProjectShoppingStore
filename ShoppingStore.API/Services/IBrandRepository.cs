using ShoppingStore.Model;

namespace ShoppingStore.API.Services
{
    public interface IBrandRepository
    {
        Task<IEnumerable<BrandModel>> GetBrandsAsync();
        Task<(IEnumerable<BrandModel>, PaginationMetadata)> GetBrandsAsync(string? searchTerm, int pageNumber, int pageSize);

        void AddBrand(BrandModel brand);

        void DeleteBrand(BrandModel brand);

        Task<BrandModel> GetBrandById(Guid brandId);
        Task<BrandModel> GetBrandBySlug(string? slug);

        Task<IEnumerable<BrandModelTest>> GetBrandsByOrderDetailCalculationAsync();
        Task<(IEnumerable<BrandModel>, PaginationMetadata)> GetBrandsByOrderDetailCalculationPaginateAsync(int pageNumber, int pageSize);

        Task<bool> SaveChangesAsync();
    }
}
