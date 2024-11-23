using Microsoft.EntityFrameworkCore;
using ShoppingStore.API.DbContexts;
using ShoppingStore.Model;
using ShoppingStore.Model.Dtos;

namespace ShoppingStore.API.Services
{
	public class CompareRepository : ICompareRepository
	{
		private readonly ShoppingStoreContext _context;
		private readonly IdentityDbContext _identityContext;
		public CompareRepository(ShoppingStoreContext context, IdentityDbContext identityContext)
		{
			_context = context ?? throw new ArgumentException(nameof(context));
			_identityContext = identityContext ?? throw new ArgumentException(nameof(identityContext));
		}
		public void AddCompare(CompareModel compare)
		{
			_context.Compares.Add(compare);
		}


		public void DeleteCompare(CompareModel compare)
		{
			_context.Compares.Remove(compare);
		}
		public async Task<bool> CheckCompareByUserIdAndProductId(Guid userId, Guid productId)
		{
			return await _context.Compares.AnyAsync(c => c.UserId == userId && c.ProductId == productId);
		}

		public async Task<CompareModel?> GetCompareById(Guid compareId)
		{
			return await _context.Compares.FirstOrDefaultAsync(c => c.Id == compareId);
		}

		public async Task<IEnumerable<CompareWithProductAndUser>> GetCompareWithProductAndUser(Guid userSubjectId)
		{
			// Cannot use multiple context instances within a single query execution   
			//var wishlist_product = await (from w in _context.Wishlists
			//                              join p in _context.Products on w.ProductId equals p.Id
			//                              join u in _identityContext.Users on w.UserId.ToString() equals u.Subject
			//                              select new WishlistWithProductAndUser() { User = u, Product = p, Wishlist = w }).ToListAsync();
			var user = await _identityContext.Users.FirstOrDefaultAsync(u => u.Subject == userSubjectId.ToString());
			var compare = await _context.Compares.Where(c => c.UserId == userSubjectId).Include(w => w.Product).ToListAsync();
			List<CompareWithProductAndUser> result = new List<CompareWithProductAndUser>();
			for (int i = 0; i < compare.Count(); i++)
			{
				result.Add(new CompareWithProductAndUser() { User = user, Compare = compare[i], Product = compare[i].Product });
			}
			return result;
		}

		public async Task<bool> SaveChangesAsync()
		{
			return (await _context.SaveChangesAsync() >= 0);
		}
	}
}
