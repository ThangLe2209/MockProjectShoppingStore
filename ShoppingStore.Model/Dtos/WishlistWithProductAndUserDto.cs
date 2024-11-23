using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShoppingStore.Model.Dtos
{
    public class WishlistWithProductAndUserDto
    {
        public WishlistDto Wishlist { get; set; }

        public ProductDto Product { get; set; } // can use ProductDto or ProductWithoutWishlistRatingDto

		public UserDto User { get; set; }

    }
}
