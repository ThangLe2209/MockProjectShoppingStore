using System.ComponentModel.DataAnnotations.Schema;

namespace ShoppingStore.Model
{
	public class OrderDetails : BaseEntity
    {
		public Guid Id { get; set; }
		public string OrderCode { get; set; }
		public string UserName { get; set; }

		public Guid ProductId { get; set; }
		public decimal Price { get; set; }
		public int Quantity { get; set; }

		[ForeignKey("ProductId")]
		public ProductModel? Product { get; set; }
	}
}
