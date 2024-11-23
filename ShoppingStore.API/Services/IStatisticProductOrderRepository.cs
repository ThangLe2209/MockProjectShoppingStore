using ShoppingStore.Model;

namespace ShoppingStore.API.Services
{
	public interface IStatisticProductOrderRepository
    {
        Task<IEnumerable<StatisticalProductOrderModel>?> GetStatisticalProductOrdersByOrderCodeAsync(string orderCode);
        void AddStatisticalProductOrderForCreations(IEnumerable<StatisticalProductOrderModel> statisticalProductOrderForCreations);
        Task<bool> SaveChangesAsync();
    }
}
