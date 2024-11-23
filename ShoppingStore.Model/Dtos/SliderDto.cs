using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using ShoppingStore.Model.Repository.Validation;

namespace ShoppingStore.Model
{
	public class SliderDto : BaseEntity
	{
		[Key]
		public Guid Id { get; set; }

		public required string Name { get; set; }

		public string? Description { get; set; }

		public int? Status { get; set; }

		public string Image { get; set; } = "noimage.jpg";
	}
}
