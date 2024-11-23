using ShoppingStore.Model.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShoppingStore.Model.Dtos
{
	public class UserForCreationDto
    {
        [Required(ErrorMessage = "Input Username")]
        public string? UserName { get; set; }

        [Required(ErrorMessage = "Input Password")]
		public string? Password { get; set; }

        [Required(ErrorMessage = "Input Email")]
		public string? Email { get; set; }

        [Required(ErrorMessage = "Choose one Role")]
        public Guid UserRoleId { get; set; }

        [Required(ErrorMessage = "Choose one Country")]
        public string? Country { get; set; }

        [MaxLength(200)]
        public string? SecurityCode { get; set; }

        public DateTime SecurityCodeExpirationDate { get; set; }
    }
}
