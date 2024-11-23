using ShoppingStore.Model.Entities;
using System.Threading.Tasks;

namespace ShoppingStore.API.Services
{
    public interface IUserRepository
    {
        Task<IEnumerable<User>> GetUsersAsync();
        Task<User> GetUserById(Guid userId, string? type);
        object AddUser(User user, string password);

        Task<bool> ActivateUserAsync(string securityCode);
        Task DeleteUserClaim(Guid userId);

        void DeleteUser(User user);

        Task<bool> IsUsernameOrEmailExist(Guid userId, string userName, string userEmail);

        void HashUserPassword(User user, string password);

        Task<User?> GetUserByEmailAsync(string userEmail);

        Task<bool> SaveChangesAsync();
    }
}