using System.ComponentModel.DataAnnotations;

namespace ShoppingStore.Model.Dtos
{
	public class ShippingForCreationDto
    {
        public required decimal Price { get; set; }
        public required string Ward { get; set; }
        public required string District { get; set; }
        public required string City { get; set; }

    }
}
