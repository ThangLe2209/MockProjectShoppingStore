using Duende.IdentityServer.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Web;
using Thang.IDP.Services;

namespace Thang.IDP.Pages.User.ResetPassword
{
    [AllowAnonymous]
    [SecurityHeaders]// built in IdentityServer, it adds security-related headers to the response
    public class IndexModel : PageModel
    {
        private readonly ILocalUserService _localUserService;
        private readonly IIdentityServerInteractionService _interaction;

        public IndexModel(ILocalUserService localUserService,
        IIdentityServerInteractionService interaction)
        {
            _localUserService = localUserService ?? throw new ArgumentNullException(nameof(localUserService));
            _interaction = interaction ?? throw new ArgumentNullException(nameof(interaction));
        }

        [BindProperty] // ensure that values from this input model can be bound to fields in our view
        public InputModel Input { get; set; }
        public async Task<IActionResult> OnGet(string ReturnUrl, string userEmail, string securityCode) // query string from url when user click btn in login page
        {
            bool checValidSecurityCodeUser = await _localUserService.ActivateUserAsync(securityCode);
            if (checValidSecurityCodeUser == false)
            {
                return Redirect($"~/User/ResetPasswordInformation?ReturnUrl={HttpUtility.UrlEncode(ReturnUrl)}&message={"Request expired time!"}");
            }
            await _localUserService.SaveChangesAsync();
            BuildModel(ReturnUrl, userEmail, securityCode); //set Url for input model when user acces page (get method)

            return Page();
        }

        private async Task BuildModel(string returnUrl, string userEmail, string securityCode)
        {
            Input = new InputModel
            {
                ReturnUrl = returnUrl,
                UserEmail = userEmail,
                SecurityCode = securityCode,
            };
        }

        public async Task<IActionResult> OnPost() // auto trigger when user post a form
        {
            if (!ModelState.IsValid)
            {
                //something when wrong, show form with error
                BuildModel(Input.ReturnUrl, Input.UserEmail, Input.SecurityCode);
                return Page();
            }

            var user = await _localUserService.GetUserByEmailAsync(Input.UserEmail);
            _localUserService.HashUserPassword(user, Input.NewPassword);
            await _localUserService.SaveChangesAsync();
            return Redirect($"~/User/ResetPasswordInformation?ReturnUrl={HttpUtility.UrlEncode(Input.ReturnUrl)}&message={"Reset password successfully!"}");
        }
    }
}
