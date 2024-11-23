using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ShoppingStore.API.DbContexts;
using ShoppingStore.Model.Entities;
using System.Security.Cryptography;

namespace ShoppingStore.API.Services
{
    public class UserRepository : IUserRepository
    {
        private readonly IdentityDbContext _context;
        private readonly IPasswordHasher<User> _passwordHasher;
        public UserRepository(IdentityDbContext context, IPasswordHasher<User> passwordHasher)
        {
            _context = context ?? throw new ArgumentException(nameof(context));
            _passwordHasher = passwordHasher ?? throw new ArgumentException(nameof(passwordHasher));
        }

        public async Task<bool> ActivateUserAsync(string securityCode)
        {
            if (string.IsNullOrWhiteSpace(securityCode))
            {
                throw new ArgumentNullException(nameof(securityCode));
            }
            // find an user with this security code as an active security code.  
            var user = await _context.Users.FirstOrDefaultAsync(u =>
                u.SecurityCode == securityCode
            && u.SecurityCodeExpirationDate >= DateTime.UtcNow
            );

            if (user == null)
            {
                return false;
            }

            user.Active = true;
            user.SecurityCode = null;
            return true;
        }

        public object AddUser(User userToAdd, string password)
        {
            if (_context.Users.Any(u => u.UserName == userToAdd.UserName))
            {
                // in a real-life scenario you'll probably want to return this as a validation issue
                return "Username must be unique";
            }

            if (_context.Users.Any(u => u.Email == userToAdd.Email))
            {
                // in a real-life scenario you'll probably want to return this as a validation issue
                return "Email must be unique";
            }

            userToAdd.SecurityCode = Convert.ToBase64String(
                RandomNumberGenerator.GetBytes(128));
            userToAdd.SecurityCodeExpirationDate = DateTime.UtcNow.AddHours(1);

            // hash & salt the password
            userToAdd.Password = _passwordHasher.HashPassword(userToAdd, password);
            _context.Users.Add(userToAdd);
            return true;
        }

        public async Task DeleteUserClaim(Guid userId)
        {
            var userClaims = await _context.UserClaims.Where(u => u.UserId == userId).ToListAsync();
            _context.UserClaims.RemoveRange(userClaims);
        }

        public void DeleteUser(User user)
        {
            _context.Users.Remove(user);
        }

        public async Task<User> GetUserById(Guid userId, string? type)
        {
            if (type == null)
            {
                return await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            }
            return await _context.Users
                .Include(u => u.Claims.Where(c => c.Type == type))
                .FirstOrDefaultAsync(u => u.Id == userId);
        }

        public async Task<IEnumerable<User>> GetUsersAsync()
        {
            return await _context.Users.Include(u => u.Claims).Include(u => u.UserRole).ToListAsync();
        }

        public async Task<User?> GetUserByEmailAsync(string userEmail)
        {
            return await _context.Users.Include(u => u.Claims)
                .Include(u => u.UserRole).FirstOrDefaultAsync(u => u.Email == userEmail);
        }

        public void HashUserPassword(User user, string password)
        {
            // hash & salt the password
            user.Password = _passwordHasher.HashPassword(user, password);
        }

        public async Task<bool> IsUsernameOrEmailExist(Guid userId, string userName, string userEmail)
        {
            var checkUserName = await _context.Users.AnyAsync(u => u.UserName == userName && u.Id != userId);
            if (checkUserName == true) return true;
            return await _context.Users.AnyAsync(u => u.Email == userEmail && u.Id != userId);
        }

        public async Task<bool> SaveChangesAsync()
        {
            return (await _context.SaveChangesAsync() >= 0);
        }
    }
}
