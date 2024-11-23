using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ShoppingStore.Model.Dtos
{
    public class StatisticalProductOrderForCreationDto
    {
        public string UserEmail { get; set; }

        public string OrderCode { get; set; }

        public Guid ProductId { get; set; }
        public decimal ProductPrice { get; set; }
        public int Quantity { get; set; }
        public decimal TotalRevenue { get; set; } // Product Price * Quantity

        public string? CouponCode { get; set; }
    }
}
