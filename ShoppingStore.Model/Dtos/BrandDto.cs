using System.ComponentModel.DataAnnotations;

namespace ShoppingStore.Model.Dtos
{
	public class BrandDto: BaseEntity
	{
		public Guid Id { get; set; }

		public required string Name { get; set; }

		public string? Description { get; set; }

		public string? Slug { get; set; }

		public int Status { get; set; }

		//public ICollection<ProductDto> Products { get; set; } = new List<ProductDto>();
		public ICollection<ProductWithoutBrandDto> Products { get; set; } = new List<ProductWithoutBrandDto>();
	}
}
