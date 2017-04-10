using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using LetMeKnowApi.Options;
using LetMeKnowApi.ViewModels;
using LetMeKnowApi.Data.Abstract;
using LetMeKnowApi.Model;
using LetMeKnowApi.Core;

namespace LetMeKnowApi.Controllers
{
    //[Route("api/[controller]")]
    public class AccountsController : Controller
    {
        private IUserRepository _userRepository;              
        private readonly JwtIssuerOptions _jwtOptions;        
        private readonly JsonSerializerSettings _serializerSettings;
        private readonly ILogger _logger;

        public AccountsController(IOptions<JwtIssuerOptions> jwtOptions, 
                                    ILoggerFactory loggerFactory,
                                    IUserRepository userRepository                                    
                                    )
        {
            _jwtOptions = jwtOptions.Value;
            ThrowIfInvalidOptions(_jwtOptions);

            _logger = loggerFactory.CreateLogger<AccountsController>();

            _serializerSettings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented
            };

            _userRepository = userRepository;             
        }        

        [HttpPost]
        [AllowAnonymous]
        [Route("signin")]
        public async Task<IActionResult> Login([FromForm] LoginViewModel user)
        {
            if (!ModelState.IsValid)
            {                            
                return BadRequest(ModelState);
            }

            User _userDb = _userRepository.GetSingle(u => u.UserName == user.UserName);   

            if (_userDb == null)
            {
                return NotFound();
            }  
            
            string passwordHashed = Extensions.EncryptPassword(user.Password, _userDb.Salt);
            if (_userDb.PasswordHash != passwordHashed){
                return BadRequest("Las contraseñas n");            
            }

            // get the user's role and give to GetClaimsIdentity as argument
            
            var identity = await GetClaimsIdentity(user);
            if (identity == null)
            {
                _logger.LogInformation($"Nombre de usuario ({user.UserName}) o contraseña ({user.Password}) inválidos");
                return BadRequest("Credenciales inválidas");
            }

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, await _jwtOptions.JtiGenerator()),
                new Claim(JwtRegisteredClaimNames.Iat, ToUnixEpochDate(_jwtOptions.IssuedAt).ToString(), ClaimValueTypes.Integer64),
                identity.FindFirst("DisneyCharacter")
            };

            // Create the JWT security token and encode it.
            var jwt = new JwtSecurityToken(
                issuer: _jwtOptions.Issuer,
                audience: _jwtOptions.Audience,
                claims: claims,
                notBefore: _jwtOptions.NotBefore,
                expires: _jwtOptions.Expiration,
                signingCredentials: _jwtOptions.SigningCredentials);

            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

            // Serialize and return the response
            var response = new
            {
                access_token = encodedJwt,
                expires_in = (int)_jwtOptions.ValidFor.TotalSeconds
            };

            var json = JsonConvert.SerializeObject(response, _serializerSettings);
            return new OkObjectResult(json);
        }

        private static void ThrowIfInvalidOptions(JwtIssuerOptions options)
        {
            if (options == null) throw new ArgumentNullException(nameof(options));

            if (options.ValidFor <= TimeSpan.Zero)
            {
                throw new ArgumentException("Must be a non-zero TimeSpan.", nameof(JwtIssuerOptions.ValidFor));
            }

            if (options.SigningCredentials == null)
            {
                throw new ArgumentNullException(nameof(JwtIssuerOptions.SigningCredentials));
            }

            if (options.JtiGenerator == null)
            {
                throw new ArgumentNullException(nameof(JwtIssuerOptions.JtiGenerator));
            }
        }

        /// <returns>Date converted to seconds since Unix epoch (Jan 1, 1970, midnight UTC).</returns>
        private static long ToUnixEpochDate(DateTime date)
        => (long)Math.Round((date.ToUniversalTime() - new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero)).TotalSeconds);

        /// <summary>
        /// IMAGINE BIG RED WARNING SIGNS HERE!
        /// You'd want to retrieve claims through your claims provider
        /// in whatever way suits you, the below is purely for demo purposes!
        /// </summary>
        private static Task<ClaimsIdentity> GetClaimsIdentity(LoginViewModel user)
        {                    
            if (user.UserName == "MickeyMouse" &&
                user.Password == "MickeyMouseIsBoss123")
            {
                return Task.FromResult(new ClaimsIdentity(new GenericIdentity(user.UserName, "Token"),
                new[]
                {
                    new Claim("DisneyCharacter", "IAmMickey")
                }));
            }

            if (user.UserName == "NotMickeyMouse" &&
                user.Password == "NotMickeyMouseIsBoss123")
            {
                return Task.FromResult(new ClaimsIdentity(new GenericIdentity(user.UserName, "Token"),
                new Claim[] { }));
            }

            // Credentials are invalid, or account doesn't exist
            return Task.FromResult<ClaimsIdentity>(null);
        }    
    }
}
