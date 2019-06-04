using System;
using System.Security.Claims;
using System.Threading.Tasks;

using IdentityModel;
using IdentityServer4.Models;
using IdentityServer4.Validation;

namespace SampleAuthServer.Configurations
{
    public class ResourceOwnerPasswordValidator : IResourceOwnerPasswordValidator
    {
        public async Task ValidateAsync(ResourceOwnerPasswordValidationContext context)
        {
            try
            {
                await Task.Run(() =>
                {
                    var inputUserName = context.UserName;
                    var inputPassword = context.Password;

                    if (inputUserName.ToLowerInvariant().Equals("username") || inputPassword.ToLowerInvariant().Equals("password"))
                    {
                        var userId = Guid.NewGuid().ToString();

                        context.Result = new GrantValidationResult(
                            subject: Guid.NewGuid().ToString(),
                            authenticationMethod: "custom",
                            claims: GetUserClaims(userId, context.UserName, "other user info here"));

                        return;
                    }

                    context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant, "Incorrect user name or password.");
                    return;
                });
            }
            catch
            {
                context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant, "Invalid username or password");
            }
        }

        public static Claim[] GetUserClaims(string id, string name, string otherInfo)
        {
            return new Claim[]
            {
                new Claim("user_id", id ?? string.Empty),
                new Claim(JwtClaimTypes.Name, name ?? string.Empty),
                // new Claim(JwtClaimTypes.GivenName, user.Firstname ?? ""),
                // new Claim(JwtClaimTypes.FamilyName, user.Lastname  ?? ""),
                // new Claim(JwtClaimTypes.Email, user.Email  ?? ""),
                new Claim("some_claim_you_want_to_see", otherInfo ?? ""),

                new Claim(JwtClaimTypes.Role, "user_role")
            };
        }
    }
}
