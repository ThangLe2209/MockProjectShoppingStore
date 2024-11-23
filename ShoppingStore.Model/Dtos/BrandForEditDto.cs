using System.ComponentModel.DataAnnotations;

namespace ShoppingStore.Model.Dtos
{
	public class BrandForEditDto
    {
		public required string Name { get; set; }

		public string? Description { get; set; }

		public string? Slug { get; set; }

		public int Status { get; set; }

	}
}
