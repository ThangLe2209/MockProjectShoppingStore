using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShoppingStore.Model.Dtos
{
    public class CompareDto : BaseEntity
    {
        [Key]
        public Guid Id { get; set; }

        public Guid ProductId { get; set; }

        public ProductWithoutWishlistRatingDto? Product { get; set; }
        public Guid UserId { get; set; } // Property string Subject in User table => not FK key

    }
}
