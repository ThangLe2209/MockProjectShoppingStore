namespace ShoppingStore.Model.Dtos
{
	public class OrderDto : BaseEntity
    {
		public Guid Id { get; set; }
		public string OrderCode { get; set; }
        public decimal ShippingCost { get; set; }
        public string? CouponCode { get; set; }
        public string UserName { get; set; }
		//public DateTime CreatedDate { get; set; }
		public int Status { get; set; }

        public string? PaymentMethod { get; set; }
    }
}
