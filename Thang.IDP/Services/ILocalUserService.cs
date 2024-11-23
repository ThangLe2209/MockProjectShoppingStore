using System.Security.Claims;
using Thang.IDP.Entities;

namespace Thang.IDP.Services
{
    public interface ILocalUserService
    {
        Task<UserSecret> GetUserSecretAsync(string subject, string name);
        Task<bool> AddUserSecret(string subject,
                string name,
                string secret);
        Task<User> GetUserByEmailAsync(string email);

        Task AddExternalProviderToUser(string subject,
            string provider,
            string providerIdentityKey);
        Task<User> FindUserByExternalProviderAsync(
            string provider, 
            string providerIdentityKey);

        public Task<User> AutoProvisionUser(string provider,
            string providerIdentityKey, IEnumerable<Claim> claims, string userEmail);

        Task<bool> ValidateCredentialsAsync(
             string userName,
             string password);

        Task<IEnumerable<UserClaim>> GetUserClaimsBySubjectAsync(
            string subject);

        Task<User> GetUserByUserNameAsync(
            string userName);

        Task<User> GetUserBySubjectAsync(
            string subject);

        void AddUser
            (User userToAdd,
            string password);

        Task<bool> IsUserActive(
            string subject);

        void HashUserPassword(User user, string password);
        Task<bool> ActivateUserAsync(string securityCode);

        Task<IEnumerable<UserRole>> GetUserRolesAsync();

        Task<UserRole?> GetFreeUserRolesAsync();

        Task<bool> SaveChangesAsync();
    }
}
