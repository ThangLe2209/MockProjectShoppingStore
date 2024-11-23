using ShoppingStore.Model;
using ShoppingStore.Model.Dtos;

namespace ShoppingStore.API.Services
{
    public interface IShippingRepository
    {
        Task<IEnumerable<ShippingModel>> GetShippingsAsync();
        Task<ShippingModel?> GetShippingByGeoDataAsync(string City, string District, string Ward);
        void AddShipping(ShippingModel shipping);

        void DeleteShipping(ShippingModel shipping);

        Task<ShippingModel?> GetShippingById(Guid shippingId);

        Task<bool> CheckExistingShipping(ShippingForCreationDto shipping);

        Task<bool> SaveChangesAsync();
    }
}
