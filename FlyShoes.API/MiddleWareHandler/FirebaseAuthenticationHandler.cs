using FirebaseAdmin;
using FirebaseAdmin.Auth;
using FlyShoes.Common.Constants;
using FlyShoes.Common.Models;
using FlyShoes.Core.Interfaces;
using FlyShoes.Interfaces;
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
        private IDatabaseService _databaseService;
        public FirebaseAuthenticationHandler(FirebaseApp firebaseApp, IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock,IDatabaseService databaseService) : base(options, logger, encoder, clock)
        {
            _firebaseApp = firebaseApp;
            _databaseService = databaseService;
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
                            ToClaims(firebaseToken.Claims,firebaseToken.Uid),
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

        private List<Claim> ToClaims(IReadOnlyDictionary<string, object> claims,string firebaseID)
        {
            // Gọi database lấy quyền
            var commandGetUser = $"SELECT * FROM User WHERE FirebaseID = @FirebaseID";
            var param = new Dictionary<string, object>()
            {
                {"@FirebaseID", firebaseID}
            };
            var user = _databaseService.QuerySingleUsingCommanText<User>(commandGetUser,param);
            _databaseService.CurrentUser = user;

            return new List<Claim>()
            {
                new Claim("id", claims["user_id"].ToString()),
                new Claim("email", claims["email"].ToString()),
                new Claim(ClaimTypes.Role,user.IsAdmin ? RoleTypeConstant.ADMIN : RoleTypeConstant.CUSTOMER)
            };

        }
    }
}
