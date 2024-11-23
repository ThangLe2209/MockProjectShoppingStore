using Duende.IdentityServer.Extensions;
using Duende.IdentityServer.Models;
using Duende.IdentityServer.Services;
using System.Security.Claims;

namespace Thang.IDP.Services
{
    public class LocalUserProfileService : IProfileService
    {
        private readonly ILocalUserService _localUserService;

        public LocalUserProfileService(ILocalUserService localUserService) 
        {
            _localUserService = localUserService ?? throw new ArgumentNullException(nameof(localUserService));
        }
        public async Task GetProfileDataAsync(ProfileDataRequestContext context) // this method is for handle claims for DB user store not TestUser class.
        {
            var subjectId = context.Subject.GetSubjectId();
            var claimsForUser = (await _localUserService.GetUserClaimsBySubjectAsync(subjectId)).ToList();
            context.AddRequestedClaims(
                claimsForUser.Select(c => new Claim(c.Type, c.Value)).ToList());
        }

        public async Task IsActiveAsync(IsActiveContext context) // this method will run after external login(window Id success login and return subId back then we should check it it's link to db server yet it yes then set the context.Active to true and IDP will redirect to client app(video 06 folder 13)
        {
            context.IsActive = true; // temporary fix
            //var subjectId = context.Subject.GetSubjectId();
            //context.IsActive = await _localUserService.IsUserActive(subjectId);
        }
    }
}
