using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Thang.IDP.Pages.User.ForgotPassword
{
    public class InputModel
    {
        public string ReturnUrl { get; set; }

        [Required]
        [MaxLength(200)]
        [Display(Name = "Email")]
        [EmailAddress]
        public string Email { get; set; }
    }
}
