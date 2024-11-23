using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Thang.IDP.Services;

namespace Thang.IDP.Pages.User.ResetPasswordInformation
{
    [SecurityHeaders]
    [AllowAnonymous]
    public class IndexModel : PageModel
    {
        [BindProperty]
        public InputModel Input { get; set; }

        private readonly ILocalUserService _localUserService;

        public IndexModel(ILocalUserService localUserService)
        {
            _localUserService = localUserService ??
                throw new ArgumentNullException(nameof(localUserService));
        }
        public async Task<IActionResult> OnGet(string ReturnUrl, string message)
        {
            Input = new InputModel();
            ViewData["ReturnUrl"] = ReturnUrl;

            Input.Message = message;
            return Page();
        }
    }
}
