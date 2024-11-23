using ShoppingStore.Model;

namespace ShoppingStore.API.Services
{
    public interface IProductQuantityRepository
    {
        void AddProductQuantity(ProductQuantityModel productQuantity);

        void DeleteProductQuantity(ProductQuantityModel productQuantity);

        Task<ProductQuantityModel?> GetProductQuantityById(Guid productQuantityId);
        Task<IEnumerable<ProductQuantityModel>> GetProductQuantitiesByProductId(Guid productId);

        Task<bool> SaveChangesAsync();
    }
}
