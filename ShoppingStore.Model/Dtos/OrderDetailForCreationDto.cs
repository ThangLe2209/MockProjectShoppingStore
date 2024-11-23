﻿using System.ComponentModel.DataAnnotations.Schema;

namespace ShoppingStore.Model.Dtos
{
	public class OrderDetailForCreationDto
	{
		public string OrderCode { get; set; }
		public string UserName { get; set; }

		public Guid ProductId { get; set; }
		public decimal Price { get; set; }
		public int Quantity { get; set; }

	}
}
