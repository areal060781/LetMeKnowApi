using System;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using LetMeKnowApi.Options;
using Microsoft.AspNetCore.Builder;
using Microsoft.IdentityModel.Tokens;

namespace LetMeKnowApi
{
    public partial class Startup
    {
        // The secret key every token will be signed with.
        // Keep this safe on the server!

        
        //private static readonly string secretKey = "mysupersecret_secretkey!123";

        private void ConfigureAuth(IApplicationBuilder app)
        {
            //var signingKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secretKey));

            var jwtAppSettingOptions = Configuration.GetSection(nameof(JwtIssuerOptions));
            
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = jwtAppSettingOptions[nameof(JwtIssuerOptions.Issuer)],

                ValidateAudience = true,
                ValidAudience = jwtAppSettingOptions[nameof(JwtIssuerOptions.Audience)],

                ValidateIssuerSigningKey = true,
                IssuerSigningKey = _signingKey,

                RequireExpirationTime = true,
                ValidateLifetime = true,

                ClockSkew = TimeSpan.Zero
            };

            app.UseJwtBearerAuthentication(new JwtBearerOptions
            {
                AutomaticAuthenticate = true,
                AutomaticChallenge = true,
                TokenValidationParameters = tokenValidationParameters
            });            

            /*app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AutomaticAuthenticate = true,
                AutomaticChallenge = true,
                AuthenticationScheme = "Cookie",
                CookieName = "access_token",
                TicketDataFormat = new CustomJwtDataFormat(
                    SecurityAlgorithms.HmacSha256,
                    tokenValidationParameters)
            });*/
        }        
    }
}
