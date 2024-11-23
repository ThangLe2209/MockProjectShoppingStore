using ShoppingStore.Model.Dtos;
using System.ComponentModel.DataAnnotations;

namespace ShoppingStore.Client.Models.ViewModels
{
	public class ProductDetailsViewModel
	{
		public ProductDto? ProductDetails { get; set; }
		//public RatingModel RatingDetails { get; set; }

		[Required(ErrorMessage = "Input comment!")]
		public string Comment { get; set; }
		[Required(ErrorMessage = "Input name!")]
		public string Name { get; set; }
		[Required(ErrorMessage = "Input email!")]
		public string Email { get; set; }
	}
}
