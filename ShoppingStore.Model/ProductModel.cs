using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using ShoppingStore.Model.Repository.Validation;

namespace ShoppingStore.Model
{
    public class ProductModel : BaseEntity
    {
        [Key]
        public Guid Id { get; set; }

        [Required, MinLength(4, ErrorMessage = "Input Product Name")]
        public required string Name { get; set; }
        public string? Slug { get; set; }

        public string? Description { get; set; }

        [Required(ErrorMessage = "Input Product Price")]
        [Range(0.01, double.MaxValue)]
        [Column(TypeName = "decimal(8,2)")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Input Product Capital Price")]
        public decimal CapitalPrice { get; set; } // Giá vốn

        [Required(ErrorMessage = "Choose one Brand")]
        public Guid BrandId { get; set; }

        [ForeignKey("BrandId")] // depend on line 30 (search foreign key attr efcore6 - have 3 ways to use)
        public BrandModel? Brand { get; set; }

        [Required(ErrorMessage = "Choose one Category")]
        public Guid CategoryId { get; set; }

        [ForeignKey("CategoryId")] // depend on line 36
        public CategoryModel? Category { get; set; }

        public string Image { get; set; } = "noimage.jpg";

        [NotMapped] // not store to database
        [FileExtension] // Repository/Validation/FileExtensionAttribute.cs
        public IFormFile? ImageUpload { get; set; }

        public ICollection<RatingModel> Ratings { get; set; } = new List<RatingModel>();
        public ICollection<OrderDetails> OrderDetails { get; set; } = new List<OrderDetails>();

        public CompareModel Compare { get; set; }
        public WishlistModel Wishlist { get; set; }

        public int Quantity { get; set; }
        public int Sold { get; set; }

        public ICollection<ProductQuantityModel> ProductQuantities { get; set; } = new List<ProductQuantityModel>();
    }
}
