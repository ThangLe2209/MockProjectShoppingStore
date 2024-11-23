using Microsoft.EntityFrameworkCore;
using ShoppingStore.API.DbContexts;
using ShoppingStore.Model;

namespace ShoppingStore.API.Services
{
    public class ProductQuantityRepository : IProductQuantityRepository
    {
        private readonly ShoppingStoreContext _context;
        public ProductQuantityRepository(ShoppingStoreContext context)
        {
            _context = context ?? throw new ArgumentException(nameof(context));
        }
        public void AddProductQuantity(ProductQuantityModel productQuantity)
        {
            _context.ProductQuantites.Add(productQuantity);
        }

        public void DeleteProductQuantity(ProductQuantityModel productQuantity)
        {
            throw new NotImplementedException();
        }

        public async Task<ProductQuantityModel?> GetProductQuantityById(Guid productQuantityId)
        {
            return await _context.ProductQuantites.FirstOrDefaultAsync(pq => pq.Id == productQuantityId);
        }

        public async Task<IEnumerable<ProductQuantityModel>> GetProductQuantitiesByProductId(Guid productId)
        {
            return await _context.ProductQuantites.Where(pq => pq.ProductId == productId).Include(pq => pq.Product).ToListAsync();
        }

        public async Task<bool> SaveChangesAsync()
        {
            return (await _context.SaveChangesAsync() >= 0);
        }
    }
}
