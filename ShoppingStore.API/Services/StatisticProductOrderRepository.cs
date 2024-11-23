using Microsoft.EntityFrameworkCore;
using ShoppingStore.API.DbContexts;
using ShoppingStore.Model;
using ShoppingStore.Model.Dtos;

namespace ShoppingStore.API.Services
{
    public class StatisticProductOrderRepository : IStatisticProductOrderRepository
    {
        private readonly ShoppingStoreContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public StatisticProductOrderRepository(ShoppingStoreContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context ?? throw new ArgumentException(nameof(context));
            _webHostEnvironment = webHostEnvironment ?? throw new ArgumentException(nameof(webHostEnvironment));
        }

        public async Task<IEnumerable<StatisticalProductOrderModel>?> GetStatisticalProductOrdersByOrderCodeAsync(string orderCode)
        {
            return await _context.StatisticalProductOrders.Where(spo => spo.OrderCode == orderCode).ToListAsync();
        }
        public void AddStatisticalProductOrderForCreations(IEnumerable<StatisticalProductOrderModel> statisticalProductOrderForCreations)
        {
            _context.StatisticalProductOrders.AddRange(statisticalProductOrderForCreations);
        }


        public async Task<bool> SaveChangesAsync()
        {
            return (await _context.SaveChangesAsync() >= 0);
        }
    }
}
