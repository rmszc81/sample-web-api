using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using IdentityServer4.Models;
using IdentityServer4.Services;

namespace SampleAuthServer.Configurations
{
    public class ProfileService : IProfileService
    {
        public async Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            try
            {
                if (!string.IsNullOrEmpty(context.Subject.Identity.Name))
                {
                    var userName = context.Subject.Identity.Name;
                    if (!string.IsNullOrEmpty(userName))
                    {
                        var userId = Guid.NewGuid().ToString();
                        var claims = ResourceOwnerPasswordValidator.GetUserClaims(userId, userName, "another info here");

                        context.IssuedClaims = claims.Where(x => context.RequestedClaimTypes.Contains(x.Type)).ToList();
                    }
                }
                else
                {
                    var userId = context.Subject.Claims.FirstOrDefault(x => x.Type == "sub");
                    var parsed = Guid.TryParse(userId.Value, out var guid);
                    if (parsed)
                    {
                        var claims = ResourceOwnerPasswordValidator.GetUserClaims(userId.Value, "user name", "some other info");
                        context.IssuedClaims = claims.Where(x => context.RequestedClaimTypes.Contains(x.Type)).ToList();
                    }
                }
            }
            catch
            {
                // do nothing //
            }
        }

        public async Task IsActiveAsync(IsActiveContext context)
        {
            try
            {
                var userId = context.Subject.Claims.FirstOrDefault(x => x.Type == "user_id");
                var parsed = Guid.TryParse(userId.Value, out var guid);
                if (parsed)
                    context.IsActive = true;
            }
            catch
            {
                // do nothing //
            }
        }
    }
}
