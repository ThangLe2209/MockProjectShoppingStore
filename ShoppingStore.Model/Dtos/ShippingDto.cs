using System.ComponentModel.DataAnnotations;

namespace ShoppingStore.Model.Dtos
{
	public class ShippingDto
    {
        public Guid Id { get; set; }

        public decimal Price { get; set; }

        public string Ward { get; set; }
        public string District { get; set; }
        public string City { get; set; }

    }
}
