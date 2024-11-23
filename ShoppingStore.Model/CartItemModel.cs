using ShoppingStore.Model;
using ShoppingStore.Model.Dtos;
using System.ComponentModel.DataAnnotations;

namespace ShoppingStore.Models
{
	public class CartItemModel
	{
		[Key]
		public Guid ProductId { get; set; }

		[Required, MinLength(4, ErrorMessage = "Yeu cau nhap ten thuong hieu")]
		public string ProductName { get; set; }

		public int Quantity { get; set; }
		public decimal Price { get; set; }
		public decimal Total
		{
			get { return Quantity * Price; }
		}

		public string Image { get; set; }

		public CartItemModel() 
		{

		}
		public CartItemModel(ProductDto product) 
		{
			ProductId = product.Id;
			ProductName = product.Name;
			Price = product.Price;
			Quantity = 1;
			Image = product.Image;
		}
	}
}
