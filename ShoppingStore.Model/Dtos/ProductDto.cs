namespace ShoppingStore.Model.Dtos
{
    public class ProductDto: BaseEntity
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;

        public string? Slug { get; set; }
        public string? Description { get; set; }

        public decimal Price { get; set; }
        public decimal CapitalPrice { get; set; } // Giá vốn

        public Guid? CategoryId { get; set; }

        public CategoryDto? Category { get; set; }

        public Guid? BrandId { get; set; }

        public BrandWithoutProductDto? Brand { get; set; }

        public string Image { get; set; } = "noimage.jpg";

        public ICollection<RatingDto> Ratings { get; set; } = new List<RatingDto>();
        public CompareDto Compare { get; set; }
        public WishlistDto Wishlist { get; set; }

        public int Quantity { get; set; }
        public int Sold { get; set; }

        public ICollection<ProductQuantityDto> ProductQuantities { get; set; } = new List<ProductQuantityDto>();
    }
}
