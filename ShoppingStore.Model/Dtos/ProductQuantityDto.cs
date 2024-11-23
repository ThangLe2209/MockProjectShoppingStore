using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShoppingStore.Model.Dtos
{
    public class ProductQuantityDto : BaseEntity
    {
        [Key]
        public Guid Id { get; set; }

        public required int Quantity { get; set; }

        public Guid ProductId { get; set; }

        [ForeignKey("ProductId")]
        public ProductWithoutProductQuantityDto? Product { get; set; }

    }
}
