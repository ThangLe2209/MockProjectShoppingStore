using ShoppingStore.Model;
using ShoppingStore.Model.Dtos;

namespace ShoppingStore.API.Services
{
    public interface IWishlistRepository
    {

        Task<bool> CheckWishlistByUserIdAndProductId(Guid userId, Guid productId);

		void AddWishlist(WishlistModel wishlist);
        void DeleteWishlist(WishlistModel wishlist);

        Task<WishlistModel?> GetWishlistById(Guid wishlistId);
        Task<IEnumerable<WishlistWithProductAndUser>> GetWishlistWithProductAndUser(Guid userSubjectId);

        Task<bool> SaveChangesAsync();
    }
}
