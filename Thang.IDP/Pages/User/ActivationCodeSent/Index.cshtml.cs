using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Web;

namespace Thang.IDP.Pages.User.ActivationCodeSent
{
    [SecurityHeaders]
    [AllowAnonymous]
    public class IndexModel : PageModel
    {
        public void OnGet(string ReturnUrl, string securityCode)
        {
            ViewData["ActivationLink"] = $"/user/activation/index?ReturnUrl={HttpUtility.UrlEncode(ReturnUrl)}&securityCode={HttpUtility.UrlEncode(securityCode)}";
        }
    }
}
