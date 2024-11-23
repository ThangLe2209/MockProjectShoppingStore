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
    public class ContactForEditDto
    {
        [Required(ErrorMessage = "Input Website Name"), MinLength(4, ErrorMessage = "Min Website Name is 4 characters")]
        public required string Name { get; set; }

        [Required(ErrorMessage = "Input Website Map")]
        public required string Map { get; set; }
        [Required(ErrorMessage = "Input Website Email")]
        public required string Email { get; set; }

        [Required(ErrorMessage = "Input Website Phone")]
        public required string Phone { get; set; }

        public string? Description { get; set; }

        public string LogoImg { get; set; } = "noimage.jpg";

        [NotMapped] // not store to database
        [FileExtension] // Repository/Validation/FileExtensionAttribute.cs
        public IFormFile? ImageUpload { get; set; }

        [NotMapped] // not store to database
        public string? ImageUrl { get; set; }
    }
}
