using Duende.IdentityServer.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Cryptography;
using System.Web;
using Thang.IDP.Pages.Login;
using Thang.IDP.Services;

namespace Thang.IDP.Pages.User.ForgotPassword
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
        public IActionResult OnGet(string returnUrl) // query string from url when user click btn in login page
        {
            BuildModel(returnUrl); //set Url for input model when user acces page (get method)
            return Page();
        }

        private async Task BuildModel(string returnUrl)
        {
            Input = new InputModel
            {
                ReturnUrl = returnUrl
            };
        }

        public async Task<IActionResult> OnPost() // auto trigger when user post a form
        {
            if (!ModelState.IsValid)
            {
                //something when wrong, show form with error
                BuildModel(Input.ReturnUrl);
                return Page();
            }

            var user = await _localUserService.GetUserByEmailAsync(Input.Email);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Email not found!");
                BuildModel(Input.ReturnUrl);
                return Page();
            }

            if (user.Active == false) {
                ModelState.AddModelError(string.Empty, "Your account is not active! Please contact the admin");
                BuildModel(Input.ReturnUrl);
                return Page();
            }
            if (user.UserName == null && user.Password == null)
            {
                ModelState.AddModelError(string.Empty, "This account is external! Login by 3rd party so don't have password");
                BuildModel(Input.ReturnUrl);
                return Page();
            }

            user.SecurityCode = Convert.ToBase64String(
                RandomNumberGenerator.GetBytes(128));
            user.SecurityCodeExpirationDate = DateTime.UtcNow.AddHours(1);
            await _localUserService.SaveChangesAsync();

            return Redirect($"~/User/ResetPassword?ReturnUrl={HttpUtility.UrlEncode(Input.ReturnUrl)}&userEmail={Input.Email}&securityCode={HttpUtility.UrlEncode(user.SecurityCode)}");
        }
    }
}
