using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Thang.IDP.Pages.User.ResetPassword
{
    public class InputModel
    {
        public string ReturnUrl { get; set; }

        public string UserEmail { get; set; }
        public string SecurityCode { get; set; }

        [MaxLength(200)]
        [DataType(DataType.Password)]
        [Display(Name = "New Password")]
        public string NewPassword { get; set; }
    }
}
