using Microsoft.EntityFrameworkCore;
using ShoppingStore.API.DbContexts;
using ShoppingStore.Model;
using ShoppingStore.Model.Dtos;

namespace ShoppingStore.API.Services
{
    public class ShippingRepository : IShippingRepository
    {
        private readonly ShoppingStoreContext _context;
        public ShippingRepository(ShoppingStoreContext context)
        {
            _context = context ?? throw new ArgumentException(nameof(context));
        }

        public async Task<IEnumerable<ShippingModel>> GetShippingsAsync()
        {
            return await _context.Shippings.OrderByDescending(s => s.UpdatedDate).ToListAsync();
        }  
        public async Task<ShippingModel?> GetShippingByGeoDataAsync(string City, string District, string Ward)
        {
            return await _context.Shippings.FirstOrDefaultAsync(s => s.City == City && s.District == District && s.Ward == Ward);
        }

        public void AddShipping(ShippingModel shipping)
        {
            _context.Shippings.Add(shipping);
        }

        public void DeleteShipping(ShippingModel shipping)
        {
            _context.Shippings.Remove(shipping);
        }

        public async Task<ShippingModel?> GetShippingById(Guid shippingId)
        {
            return await _context.Shippings.FirstOrDefaultAsync(s => s.Id == shippingId);
        }

        public async Task<bool> CheckExistingShipping(ShippingForCreationDto shipping)
        {
            return await _context.Shippings.AnyAsync(s => s.City == shipping.City && s.District == shipping.District && s.Ward == shipping.Ward);
        }

        public async Task<bool> SaveChangesAsync()
        {
            return (await _context.SaveChangesAsync() >= 0);
        }
    }
}
