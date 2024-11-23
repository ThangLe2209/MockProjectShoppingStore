using Duende.IdentityServer;
using Duende.IdentityServer.Models;
using Thang.IDP.Services;

namespace Thang.IDP;

public static class Config
{
    public static IEnumerable<IdentityResource> IdentityResources =>
        new IdentityResource[]
        {
            new IdentityResources.OpenId(), // support OpenId scope (must required scope for OIDC) - add claims: sub
            new IdentityResources.Profile(), // add claims: name, family_name, given_name, ... view notepad file
            new IdentityResources.Email(),
			new IdentityResource("roles", "Your role(s)" // new custom IdentityResource for custom define claim in TestUsers.cs
                , new [] { "role" }), // all claims this identity provided (key of custom claim in TestUsers.cs)
          
            new IdentityResource("country", "The country you're living in" // new custom IdentityResource for custom define claim in TestUsers.cs
                , new List<string> { "country" }) // all claims this identity provided (key of custom claim in TestUsers.cs)

        };

    public static IEnumerable<ApiResource> ApiResources =>
        new ApiResource[]
        {
            new ApiResource("shoppingstoreapi", "Shopping Store API", new [] { "role", "country", "given_name", IdentityServerConstants.StandardScopes.Email }) // third parameters is attach claims to access token
            {
                Scopes = { "shoppingstoreapi.fullaccess",
                    "shoppingstoreapi.read",
                    "shoppingstoreapi.write" },
                ApiSecrets = { new Secret("apisecret".Sha256()) } // secret key for BE use to get info when using reference token(be will get claim each request by this key send to Introspection IDP endpoint - view notepad) (line 49: AccessTokenType = AccessTokenType.Reference)
            }
        };
    public static IEnumerable<ApiScope> ApiScopes =>
        new ApiScope[]
            {
                new ApiScope("shoppingstoreapi.fullaccess"),
                new ApiScope("shoppingstoreapi.read"),
                new ApiScope("shoppingstoreapi.write")
            };

    public static IEnumerable<Client> Clients =>
        new Client[]
            {
                new Client()
                {
                    ClientName = "Shopping Store",
                    ClientId = "shoppingstoreclient",
                    AllowedGrantTypes = GrantTypes.Code, // Authorization Code Flow (return login page for client to login - if m2m(API-IDP, consoleapp-IDP,...) use GrantTypes.ClientCredentials)
                    AccessTokenType = AccessTokenType.Reference, // support reference token(access token will no longer directly access view log FE when login), BE(API) will call to Intropection endpoint(IDP) to get claim content
                    AllowOfflineAccess = true, // support refresh token (view video 09/refreshtoken.mp4 to change lifetime), offline mean user no longer logged in to the IDP (access token timeup)
                    UpdateAccessTokenClaimsOnRefresh = true, // get new claim info when refresh
                    AccessTokenLifetime = 120,
                    RedirectUris = // login page code redirect to client in this uri - host is FE address/signin-oidc
                    {
                        //"https://localhost:7239/signin-oidc"
                        $"{ConfigurationHelper.config["FEPort"]}/signin-oidc"
                    },
                    PostLogoutRedirectUris = //logout url redirect
                    {
                        //"https://localhost:7239/signout-callback-oidc"
                        $"{ConfigurationHelper.config["FEPort"]}/signout-callback-oidc"
                    },
                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.Email,
                        "roles", // custom identityresource - to get custom claims in client
                        //"imagegalleryapi.fullaccess",
                        "shoppingstoreapi.read",
                        "shoppingstoreapi.write",
                        "country"
                    },
                    ClientSecrets =  // allow the client application to execute an authenticated call to the token endpoint
					{
                        new Secret("secret".Sha256())
                    },
                    //RequireConsent = true, //show up consent page after sucessful login (show scope user have - want register)
                } ,
     //           new Client() // m-m client
     //           {
     //               ClientName = "Image Gallery",
     //               ClientId = "imagegalleryapi",
     //               AllowedGrantTypes = GrantTypes.Code, // Authorization Code Flow
     //               RedirectUris = // login page code redirect to client in this uri - host is FE address/signin-oidc
     //               {
     //                   "https://localhost:7184/signin-oidc"
     //               },
     //               PostLogoutRedirectUris = //logout url redirect
     //               {
     //                   "https://localhost:7184/signout-callback-oidc"
     //               },
     //               AllowedScopes =
     //               {
     //                   IdentityServerConstants.StandardScopes.OpenId,
     //                   IdentityServerConstants.StandardScopes.Profile,
     //                   "roles" // custom identityresource
     //               },
     //               ClientSecrets =  // allow the client application to execute an authenticated call to the token endpoint
					//{
     //                   new Secret("secret".Sha256())
     //               },
     //               RequireConsent = true, //show up consent page after sucessful login (show scope user have - want register)
     //           }
            };
}