﻿using ShoppingStore.Models;

namespace ShoppingStore.Client.Models.ViewModels
{
	public class CartItemViewModel
	{
		public List<CartItemModel> CartItems { get; set; }
		public decimal GrandTotal { get; set; }

		public decimal ShippingCost { get; set; }
	}
}