using System.ComponentModel.DataAnnotations;

namespace ShoppingStore.Model
{
    public class CategoryModel : BaseEntity
    {
        [Key]
        public Guid Id { get; set; }

        [Required, MinLength(4, ErrorMessage = "Input Category Name")]
        public required string Name { get; set; }

        public string? Description { get; set; }

        public string? Slug { get; set; }

        public int Status { get; set; }

        public ICollection<ProductModel> Products { get; set; } = new List<ProductModel>();
    }
}