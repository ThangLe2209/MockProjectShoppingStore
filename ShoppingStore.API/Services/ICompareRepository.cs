using ShoppingStore.Model;
using ShoppingStore.Model.Dtos;

namespace ShoppingStore.API.Services
{
    public interface ICompareRepository
	{

        Task<bool> CheckCompareByUserIdAndProductId(Guid userId, Guid productId);

		void AddCompare(CompareModel compare);
        void DeleteCompare(CompareModel compare);

        Task<CompareModel?> GetCompareById(Guid compareId);
        Task<IEnumerable<CompareWithProductAndUser>> GetCompareWithProductAndUser(Guid userSubjectId);

        Task<bool> SaveChangesAsync();
    }
}
