using System.ComponentModel.DataAnnotations;

namespace ShoppingStore.Model.Dtos
{
	public class CouponDto : BaseEntity
	{
        public Guid Id { get; set; }
        public required string Name { get; set; }
		
		public required string Description { get; set; }
        public DateTime DateStart { get; set; }
        public DateTime DateExpired { get; set; }
		//public required decimal Price { get; set; }
		public required int Quantity { get; set; }
		public int Status { get; set; }
        public decimal DiscountPercent { get; set; }
        public decimal DiscountDecrease { get; set; }
    }
}
