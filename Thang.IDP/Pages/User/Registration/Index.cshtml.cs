using Duende.IdentityServer;
using Duende.IdentityServer.Services;
using IdentityModel;
using Thang.IDP.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Web;

namespace Thang.IDP.Pages.User.Registration
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
            var userList = new List<object>();
            userList.Add(new { Id = new Guid("1069eee8-509a-46f9-9800-da3d0e12d560"), Value = "FreeUser" });

            Input = new InputModel
            {
                ReturnUrl = returnUrl,
                //Roles = new SelectList((await _localUserService.GetUserRolesAsync()).Where(userRole => userRole.Value == "FreeUser"), "Id", "Value") // postgres db failed
                Roles = new SelectList(userList, "Id", "Value")
            };
        }

        public async Task<IActionResult> OnPost() // auto trigger when user post a form
        {
            if(!ModelState.IsValid)
            {
                //something when wrong, show form with error
                BuildModel(Input.ReturnUrl);
                return Page();
            }

            // create user & claims
            var userToCreate = new Entities.User
            {
                UserName = Input.UserName,
                Subject = Guid.NewGuid().ToString(),
                Email = Input.Email,
                Active = false,
                UserRoleId = new Guid(Input.Role)
            };

            userToCreate.Claims.Add(new Entities.UserClaim()
            {
                Type = "country",
                Value = Input.Country
            });

            var userRoles = await _localUserService.GetUserRolesAsync();

            userToCreate.Claims.Add(new Entities.UserClaim()
            {
                Type = "role",
                Value = userRoles.FirstOrDefault(u => u.Id.ToString() == Input.Role).Value
            });

            userToCreate.Claims.Add(new Entities.UserClaim()
            {
                Type = JwtClaimTypes.GivenName,
                Value = Input.GivenName
            });

            userToCreate.Claims.Add(new Entities.UserClaim()
            {
                Type = JwtClaimTypes.FamilyName,
                Value = Input.FamilyName
            });

			userToCreate.Claims.Add(new Entities.UserClaim()
			{
				Type = JwtClaimTypes.Email,
				Value = Input.Email
			});

			try
            {
                _localUserService.AddUser(userToCreate, Input.Password);
            }
            catch (Exception ex) 
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                BuildModel(Input.ReturnUrl);
                return Page();
            }
            await _localUserService.SaveChangesAsync();

            // create an activation link - we need an absolute URL, therefore
            // we use Url.PageLink instead of Url.Page (fake similar click from email service like Gmail,...)
            var activationLink = Url.PageLink("/user/activation/index",
                values: new { securityCode = userToCreate.SecurityCode });

            Console.WriteLine($"Activation link: {activationLink}");

            //return Redirect("~/User/ActivationCodeSent");

            return Redirect($"~/User/ActivationCodeSent?ReturnUrl={HttpUtility.UrlEncode(Input.ReturnUrl)}&securityCode={HttpUtility.UrlEncode(userToCreate.SecurityCode)}");
            //return RedirectToAction("ActivationCodeSent","User", new {activationLink = activationLink, returnUrl = Input.ReturnUrl });

            // Issue authentication cookie (log the user in)
            //var isUser = new IdentityServerUser(userToCreate.Subject)
            //{
            //    DisplayName = userToCreate.UserName
            //};
            //await HttpContext.SignInAsync(isUser);

            //// continue with the flow     
            //if (_interaction.IsValidReturnUrl(Input.ReturnUrl) || Url.IsLocalUrl(Input.ReturnUrl))
            //{
            //    return Redirect(Input.ReturnUrl);
            //}

            //return Redirect("~/");
        }
    }
}
