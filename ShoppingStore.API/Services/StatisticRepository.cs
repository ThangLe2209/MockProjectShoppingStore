using Microsoft.EntityFrameworkCore;
using ShoppingStore.API.DbContexts;
using ShoppingStore.Model;

namespace ShoppingStore.API.Services
{
    public class StatisticRepository : IStatisticRepository
    {
        private readonly ShoppingStoreContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public StatisticRepository(ShoppingStoreContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context ?? throw new ArgumentException(nameof(context));
            _webHostEnvironment = webHostEnvironment ?? throw new ArgumentException(nameof(webHostEnvironment));
        }

        public void AddStatistic(StatisticalModel statistic)
        {
            throw new NotImplementedException();
        }

        public void DeleteStatistic(StatisticalModel statistic)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<dynamic>> GetStatisticsAsync()
        {
            var data = await _context.Statisticals.Select(s => new
            {
                date = s.CreatedDate.ToString("yyyy-MM-dd"),
                sold = s.Sold,
                quantity = s.Quantity,
                revenue = s.Revenue,
                profit = s.Profit
            }).ToListAsync();
            return data;
        }

        public async Task<IEnumerable<dynamic>> GetStatisticsByStartEndDateAsync(DateTime startDate, DateTime endDate)
        {
            var data = await _context.Statisticals
            .Where(s => s.CreatedDate >= startDate && s.CreatedDate <= endDate)
            .Select(s => new
            {
                date = s.CreatedDate.ToString("yyyy-MM-dd"),
                sold = s.Sold,
                quantity = s.Quantity,
                revenue = s.Revenue,
                profit = s.Profit
            }).ToListAsync();
            return data;
        }

        public async Task<bool> SaveChangesAsync()
        {
            return (await _context.SaveChangesAsync() >= 0);
        }
    }
}
