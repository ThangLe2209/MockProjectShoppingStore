using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShoppingStore.Model.Dtos
{
    public class ProductQuantityForCreationDto
    {
        [Key]
        public Guid Id { get; set; }

        public required int Quantity { get; set; }

        public Guid ProductId { get; set; }
    }
}
