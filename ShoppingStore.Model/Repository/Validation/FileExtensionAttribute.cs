using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShoppingStore.Model.Repository.Validation
{
    public class FileExtensionAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is IFormFile file)
            {
                var extension = Path.GetExtension(file.FileName); //123.jpg
                string[] extensions = { "jpg", "png", "jpeg", "webp", "jfif" };

                bool result = extensions.Any(x => extension.ToLower().EndsWith(x));

                if (!result)
                {
                    return new ValidationResult("Allowed extensions are jpg, png or jpeg");
                }
            }
            return ValidationResult.Success;
        }
    }
}
