using Microsoft.AspNetCore.Http;
using ShoppingStore.Model.Repository.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShoppingStore.Model.Dtos
{
	public class ProductForCreationDto
	{

		[Required, MinLength(4, ErrorMessage = "Input Product Name")]
		public required string Name { get; set; }
		public string? Slug { get; set; }

		public string? Description { get; set; }

		[Required(ErrorMessage = "Input Product Price")]
		public decimal Price { get; set; }

        [Required(ErrorMessage = "Input Product Capital Price")]
        public decimal CapitalPrice { get; set; } // Giá vốn

        [Required(ErrorMessage = "Choose one Brand")]
		public Guid BrandId { get; set; }

		[Required(ErrorMessage = "Choose one Category")]
		public Guid CategoryId { get; set; }

		//public CategoryModel? Category { get; set; }


		//public BrandModel? Brand { get; set; }

		public string Image { get; set; } = "noimage.jpg";

		[NotMapped] // not store to database
		[FileExtension] // Repository/Validation/FileExtensionAttribute.cs
		public IFormFile? ImageUpload { get; set; }
	}
}
