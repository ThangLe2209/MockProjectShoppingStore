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
    public class SongModel : BaseEntity
    {
        [Key]
        public Guid Id { get; set; }

        //[Required(ErrorMessage = "Input Song Name"), MinLength(4, ErrorMessage = "Min Song Name is 4 characters")]
        public required string Name { get; set; }

        public string? Description { get; set; }

        public int? Status { get; set; }

        public string Image { get; set; } = "noimage.jpg";

        [NotMapped] // not store to database
        [FileExtension] // Repository/Validation/FileExtensionAttribute.cs
        public IFormFile? ImageUpload { get; set; }

        [NotMapped] // not store to database
        public string? ImageUrl { get; set; }
        public string Song { get; set; } = String.Empty;

        [NotMapped] // not store to database
        public IFormFile? SongUpload { get; set; }
    }
}
