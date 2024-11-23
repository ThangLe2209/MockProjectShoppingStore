// Copyright (c) Duende Software. All rights reserved.
// See LICENSE in the project root for license information.

using System.Security.Claims;
using Duende.IdentityServer;
using Duende.IdentityServer.Events;
using Duende.IdentityServer.Services;
using Duende.IdentityServer.Test;
using IdentityModel;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Thang.IDP.Services;

namespace Thang.IDP.Pages.ExternalLogin;

[AllowAnonymous]
[SecurityHeaders]
public class Callback : PageModel
{
    //private readonly TestUserStore _users;
    private readonly ILocalUserService _localUserService;
    private readonly IIdentityServerInteractionService _interaction;
    private readonly ILogger<Callback> _logger;
    private readonly IEventService _events;

    private readonly Dictionary<string, string> _facebookClaimTypeMap = new()
        {
            { "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/givenname",
            JwtClaimTypes.GivenName},
            { "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/surname",
            JwtClaimTypes.FamilyName},
            { "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress",
            JwtClaimTypes.Email}
        };

    public Callback(
        IIdentityServerInteractionService interaction,
        IEventService events,
        ILogger<Callback> logger,
        ILocalUserService localUserService,
        TestUserStore? users = null)
    {
        // this is where you would plug in your own custom identity management library (e.g. ASP.NET Identity)
        //_users = users ?? throw new InvalidOperationException("Please call 'AddTestUsers(TestUsers.Users)' on the IIdentityServerBuilder in Startup or remove the TestUserStore from the AccountController.");
        _localUserService = localUserService ?? throw new ArgumentException(nameof(localUserService));
        _interaction = interaction;
        _logger = logger;
        _events = events;
    }
        
    public async Task<IActionResult> OnGet()
    {
        // read external identity from the temporary cookie
        var result = await HttpContext.AuthenticateAsync(IdentityServerConstants.ExternalCookieAuthenticationScheme);
        if (result.Succeeded != true)
        {
            throw new InvalidOperationException($"External authentication error: { result.Failure }");
        }

        var externalUser = result.Principal ?? 
            throw new InvalidOperationException("External authentication produced a null Principal");

        if (_logger.IsEnabled(LogLevel.Debug))
        {
            var externalClaims = externalUser.Claims.Select(c => $"{c.Type}: {c.Value}");
            _logger.ExternalClaims(externalClaims);
        }

        // lookup our user and external provider info
        // try to determine the unique id of the external user (issued by the provider)
        // the most common claim type for that are the sub claim and the NameIdentifier
        // depending on the external provider, some other claim type might be used
        var userIdClaim = externalUser.FindFirst(JwtClaimTypes.Subject) ??
                          externalUser.FindFirst(ClaimTypes.NameIdentifier) ??
                          throw new InvalidOperationException("Unknown userid");

        var provider = result.Properties.Items["scheme"] ?? throw new InvalidOperationException("Null scheme in authentiation properties");
        var providerUserId = userIdClaim.Value;

        // find external user
        var user = await _localUserService.FindUserByExternalProviderAsync(provider, providerUserId); // we will just check in UserLogin table
        if (user == null) // not login by 3rd any time yet
        {
            // remove the userid claim: that information is
            // stored in the UserLogins table
            var claims = externalUser.Claims.ToList(); // claims from 3rd party
            claims.Remove(userIdClaim); // not store in UserClaims because we will already store in field ProviderIdentityKey in UserLogin Table (see line 80, 136,159)

            // different external login providers often require different
            // ways of handling 
            // provisioning / linking
            if (provider == "AAD")
            {
                // get email claim value
                var emailFromAzureAD = externalUser.Claims
                  .FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress")?.Value;

                if (!string.IsNullOrEmpty(emailFromAzureAD))
                {
                    // try to find a user with matching email
                    user = await _localUserService
                        .GetUserByEmailAsync(emailFromAzureAD);

                    // if it exists, add AAD as a provider
                    if (user != null)
                    {
                        await _localUserService.AddExternalProviderToUser(
                            user.Subject, provider, providerUserId);
                        await _localUserService.SaveChangesAsync();
                    }

                    // note: creating a new user if no match is found is
                    // a common practice - we won't do that, we already
                    // did that in our Facebook integration sample

                }
            }
            else if (provider == "Facebook")
            {
                // get email claim value
                var emailFromFacebook = externalUser.Claims
                  .FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress")?.Value;

                if (!string.IsNullOrEmpty(emailFromFacebook))
                {
                    // try to find a local user with matching email
                    user = await _localUserService
                        .GetUserByEmailAsync(emailFromFacebook);

                    // if local user with this email exists, add Facebook as a provider
                    if (user != null)
                    {
                        await _localUserService.AddExternalProviderToUser(
                            user.Subject, provider, providerUserId); // just add new line in UserLogin table with Foreign key point to this local user => claims will be this local user also => not create anything new except one line in UserLogin table
                        await _localUserService.SaveChangesAsync();
                    }
                    else
                    {
                        var mappedClaims = new List<Claim>();
                        // map the claims, and ignore those for which no
                        // mapping exists
                        foreach (var claim in claims)
                        {
                            if (_facebookClaimTypeMap.ContainsKey(claim.Type))
                            {
                                mappedClaims.Add(
                                    new Claim(_facebookClaimTypeMap[claim.Type],
                                    claim.Value));
                            }
                        }
                        mappedClaims.Add(new Claim("role", "FreeUser"));
                        mappedClaims.Add(new Claim("country", "vi"));

                        // auto-provision the user
                        user = await _localUserService.AutoProvisionUser(
                            provider, providerUserId, mappedClaims.ToList(), emailFromFacebook); // add one line User table, multiple lines UserClaims from Facebook and one line in UserLogin just by User context and navigation collection/property
                        await _localUserService.SaveChangesAsync();
                    }
                }
            }
        }

        //// find external user
        //var user = _users.FindByExternalProvider(provider, providerUserId);
        //if (user == null)
        //{
        //    // this might be where you might initiate a custom workflow for user registration
        //    // in this sample we don't show how that would be done, as our sample implementation
        //    // simply auto-provisions new external user
        //    //
        //    // remove the user id claim so we don't include it as an extra claim if/when we provision the user
        //    var claims = externalUser.Claims.ToList();
        //    claims.Remove(userIdClaim);
        //    user = _users.AutoProvisionUser(provider, providerUserId, claims.ToList());
        //}


        // this allows us to collect any additional claims or properties
        // for the specific protocols used and store them in the local auth cookie.
        // this is typically used to store data needed for signout from those protocols.
        var additionalLocalClaims = new List<Claim>();
        var localSignInProps = new AuthenticationProperties();
        CaptureExternalLoginContext(result, additionalLocalClaims, localSignInProps);
            
        // issue authentication cookie for user
        var isuser = new IdentityServerUser(user.Subject)
        {
            DisplayName = user.UserName,
            IdentityProvider = provider,
            AdditionalClaims = additionalLocalClaims
        };

        await HttpContext.SignInAsync(isuser, localSignInProps);

        // delete temporary cookie used during external authentication
        await HttpContext.SignOutAsync(IdentityServerConstants.ExternalCookieAuthenticationScheme);

        // retrieve return URL
        var returnUrl = result.Properties.Items["returnUrl"] ?? "~/";

        // check if external login is in the context of an OIDC request
        var context = await _interaction.GetAuthorizationContextAsync(returnUrl);
        await _events.RaiseAsync(new UserLoginSuccessEvent(provider, providerUserId, user.Subject, user.UserName, true, context?.Client.ClientId));
        Telemetry.Metrics.UserLogin(context?.Client.ClientId, provider!);

        if (context != null)
        {
            if (context.IsNativeClient())
            {
                // The client is native, so this change in how to
                // return the response is for better UX for the end user.
                return this.LoadingPage(returnUrl);
            }
        }

        return Redirect(returnUrl);
    }

    // if the external login is OIDC-based, there are certain things we need to preserve to make logout work
    // this will be different for WS-Fed, SAML2p or other protocols
    private static void CaptureExternalLoginContext(AuthenticateResult externalResult, List<Claim> localClaims, AuthenticationProperties localSignInProps)
    {
        ArgumentNullException.ThrowIfNull(externalResult.Principal, nameof(externalResult.Principal));

        // capture the idp used to login, so the session knows where the user came from
        localClaims.Add(new Claim(JwtClaimTypes.IdentityProvider, externalResult.Properties?.Items["scheme"] ?? "unknown identity provider"));

        // if the external system sent a session id claim, copy it over
        // so we can use it for single sign-out
        var sid = externalResult.Principal.Claims.FirstOrDefault(x => x.Type == JwtClaimTypes.SessionId);
        if (sid != null)
        {
            localClaims.Add(new Claim(JwtClaimTypes.SessionId, sid.Value));
        }

        // if the external provider issued an id_token, we'll keep it for signout
        var idToken = externalResult.Properties?.GetTokenValue("id_token");
        if (idToken != null)
        {
            localSignInProps.StoreTokens(new[] { new AuthenticationToken { Name = "id_token", Value = idToken } });
        }
    }
}
