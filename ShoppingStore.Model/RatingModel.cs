using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ShoppingStore.Model
{
    public class RatingModel : BaseEntity
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public Guid ProductId { get; set; }

        [ForeignKey("ProductId")] // depend on line 12
        public ProductModel? Product { get; set; }

        [Required(ErrorMessage = "Input Comment!")]
        public string Comment { get; set; }

        [Required(ErrorMessage = "Input Name!")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Input Email!")]
        public string Email { get; set; }

        public string Star { get; set; }
    }
}