using FirebaseAdmin;
using FirebaseAdmin.Auth;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace FlyShoes.API.FirebaseHandler
{
    public class FirebaseAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        private FirebaseApp _firebaseApp;
        public FirebaseAuthenticationHandler(FirebaseApp firebaseApp, IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock) : base(options, logger, encoder, clock)
        {
            _firebaseApp = firebaseApp;
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (Context.Request.Headers.ContainsKey("Authorization"))
            {
                try
                {
                    string token = Context.Request.Headers["Authorization"];
                    FirebaseToken firebaseToken = await FirebaseAuth.GetAuth(_firebaseApp).VerifyIdTokenAsync(token);

                    var listClaims = new List<ClaimsIdentity>() {
                        new ClaimsIdentity(
                            ToClaims(firebaseToken.Claims),
                            nameof(FirebaseAuthenticationHandler)
                        )
                    };
                    var claimsPrincipal = new ClaimsPrincipal(listClaims);
                    var firebaseTicket = new AuthenticationTicket(claimsPrincipal, JwtBearerDefaults.AuthenticationScheme);
                    return AuthenticateResult.Success(firebaseTicket);
                }
                catch (Exception ex)
                {
                    AuthenticateResult.Fail(ex);
                }
            }

            return AuthenticateResult.NoResult();
        }

        private List<Claim> ToClaims(IReadOnlyDictionary<string, object> claims)
        {
            // Gọi database lấy quyền

            return new List<Claim>()
            {
                new Claim("id", claims["user_id"].ToString()),
                new Claim("email", claims["email"].ToString()),
                new Claim(ClaimTypes.Role,"Admin")
            };

        }
    }
}
