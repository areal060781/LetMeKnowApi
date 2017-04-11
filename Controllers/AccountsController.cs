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
using System.Collections.Generic;
using System.Linq;
using AutoMapper;

namespace LetMeKnowApi.Controllers
{
    //[Route("api/[controller]")]
    public class AccountsController : Controller
    {
        private IUserRepository _userRepository; 
        private IRoleRepository _roleRepository;             
        private readonly JwtIssuerOptions _jwtOptions;        
        private readonly JsonSerializerSettings _serializerSettings;
        private readonly ILogger _logger;        

        public AccountsController(IOptions<JwtIssuerOptions> jwtOptions, 
                                    ILoggerFactory loggerFactory,
                                    IUserRepository userRepository,
                                    IRoleRepository roleRepository                                
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
            _roleRepository = roleRepository;        
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

            User _userDb = _userRepository.GetSingle(u => u.UserName == user.UserName, u => u.Roles);   

            if (_userDb == null)
            {
                _logger.LogInformation($"Nombre de usuario ({user.UserName}) o contraseña ({user.Password}) inválidos");
                return NotFound();
            }  
            
            string passwordHashed = Extensions.EncryptPassword(user.Password, _userDb.Salt);
            if (_userDb.PasswordHash != passwordHashed){
                _logger.LogInformation("La contraseña proporcionada es incorrecta");
                return BadRequest("Credenciales inválidas");                          
            }
              
            Role _roleDb = null;
            foreach(var role in _userDb.Roles)
            {
                _roleDb = _roleRepository.GetSingle(r => r.Id == role.RoleId);                
            }

            if (_roleDb == null)
            {                
                _logger.LogInformation($"El usuario ({user.UserName}) no tiene un rol asignado");
                return BadRequest("Credenciales inválidas");
            }        

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, (_userDb.Id).ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, await _jwtOptions.JtiGenerator()),
                new Claim(JwtRegisteredClaimNames.Iat, ToUnixEpochDate(_jwtOptions.IssuedAt).ToString(), ClaimValueTypes.Integer64), 
                new Claim(ClaimTypes.Role, _roleDb.Name),  
                new Claim(ClaimTypes.Name, _userDb.UserName)
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

        private bool UserNameExists(string userName, int? id)
        {                  
            if (id != null){
                return (_userRepository.GetSingle(u => u.UserName == userName, u => u.Id != id) != null) ? true : false;
            }
            else
            {
                return (_userRepository.GetSingle(u => u.UserName == userName) != null) ? true : false;
            }                                  
        }


        // POST api/users 
        [HttpPost]
        [AllowAnonymous]
        [Route("signup")]
        public IActionResult Create([FromBody]RegisterViewModel user)
        {

            if (!ModelState.IsValid)
            {                            
                return BadRequest(ModelState);
            }
            
            if (UserNameExists(user.UserName, null))
            {
                var message = new[] {"El nombre de usuario ya ha sido tomado"};
                var response = new { userName = message };                    
                return BadRequest(response);                    
            }

            string salt = Extensions.CreateSalt();
            string password = Extensions.EncryptPassword(user.Password, salt);  
            int defaultRole = 2;          

            User _newUser = new User 
            { 
                UserName = user.UserName, 
                PasswordHash = password, 
                Salt = salt, 
                Email = user.Email
            };

            _newUser.Roles.Add(new UserRole
            {
                User = _newUser,
                RoleId = defaultRole
            });

            _userRepository.Add(_newUser);
            _userRepository.Commit();
            
            UserViewModel _userVM = Mapper.Map<User, UserViewModel>(_newUser);  
            
            CreatedAtRouteResult result = CreatedAtRoute("GetUser", new { controller = "Users", id = _newUser.Id }, _userVM);
            return result;            
        }       
    }
}
