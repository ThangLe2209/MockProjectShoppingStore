using ShoppingStore.Model;

namespace ShoppingStore.API.Services
{
	public interface IStatisticRepository
    {
		Task<IEnumerable<dynamic>> GetStatisticsAsync();

        Task<IEnumerable<dynamic>> GetStatisticsByStartEndDateAsync(DateTime startDate, DateTime endDate);

        void AddStatistic(StatisticalModel statistic);

        void DeleteStatistic(StatisticalModel statistic);

        Task<bool> SaveChangesAsync();
    }
}
