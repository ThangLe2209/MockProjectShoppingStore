using ShoppingStore.Model.Entities;

namespace ShoppingStore.API.Services
{
    public interface IRoleRepository
    {
        void AddRole(UserRole role);
        void DeleteRole(UserRole role);

        Task<UserRole> GetRoleById(Guid roleId);

        Task<UserRole> GetRoleByName(string? value);

        Task<IEnumerable<UserRole>> GetRolesAsync();

        Task<bool> SaveChangesAsync();
    }
}