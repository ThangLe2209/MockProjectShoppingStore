using ShoppingStore.Model.Dtos;
using System.ComponentModel.DataAnnotations;

namespace ShoppingStore.Model
{
    public class BrandModelTest
    {
        [Key]
        public Guid Id { get; set; }

        [Required, MinLength(4, ErrorMessage = "Input brand name")]
        public required string Name { get; set; }

        public string? Description { get; set; }

        public string? Slug { get; set; }

        public int Status { get; set; }

        //public decimal Price { get; set; }

        // Navigation(Property/Collection) not supported when use .SqlQuery()
        //public ICollection<ProductDto> Products { get; set; } = new List<ProductDto>();
        public int? SumBrandQuantity { get; set; }
    }
}