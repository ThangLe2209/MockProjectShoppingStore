using Microsoft.EntityFrameworkCore;
using ShoppingStore.API.DbContexts;
using ShoppingStore.Model;
using ShoppingStore.Model.Dtos;

namespace ShoppingStore.API.Services
{
    public class WishlistRepository: IWishlistRepository
    {
        private readonly ShoppingStoreContext _context;
        private readonly IdentityDbContext _identityContext;
        public WishlistRepository(ShoppingStoreContext context, IdentityDbContext identityContext)
        {
            _context = context ?? throw new ArgumentException(nameof(context));
            _identityContext = identityContext ?? throw new ArgumentException(nameof(identityContext));
        }
		public void DeleteWishlist(WishlistModel wishlist)
		{
			_context.Wishlists.Remove(wishlist);
		}

		public void AddWishlist(WishlistModel wishlist)
        {
            _context.Wishlists.Add(wishlist);
        }

        public async Task<WishlistModel?> GetWishlistById(Guid wishlistId)
        {
            return await _context.Wishlists.FirstOrDefaultAsync(w => w.Id == wishlistId);
        }

        public async Task<IEnumerable<WishlistWithProductAndUser>> GetWishlistWithProductAndUser(Guid userSubjectId)
        {
            // Cannot use multiple context instances within a single query execution   
            //var wishlist_product = await (from w in _context.Wishlists
            //                              join p in _context.Products on w.ProductId equals p.Id
            //                              join u in _identityContext.Users on w.UserId.ToString() equals u.Subject
            //                              select new WishlistWithProductAndUser() { User = u, Product = p, Wishlist = w }).ToListAsync();
            var user = await _identityContext.Users.FirstOrDefaultAsync(u => u.Subject == userSubjectId.ToString());
            var wishlist = await _context.Wishlists.Where(w => w.UserId == userSubjectId).Include(w => w.Product).ToListAsync();
            List<WishlistWithProductAndUser> result = new List<WishlistWithProductAndUser>();
            for(int i = 0; i < wishlist.Count(); i++)
            {
                result.Add(new WishlistWithProductAndUser() { User = user, Wishlist = wishlist[i], Product = wishlist[i].Product });
            }
            return result;
        }

		public async Task<bool> SaveChangesAsync()
        {
            return (await _context.SaveChangesAsync() >= 0);
        }

		public async Task<bool> CheckWishlistByUserIdAndProductId(Guid userId, Guid productId)
		{
			return await _context.Wishlists.AnyAsync(w => w.UserId == userId && w.ProductId == productId);
		}
	}
}
