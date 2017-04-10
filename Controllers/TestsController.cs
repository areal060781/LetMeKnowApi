using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Security.Claims;
using LetMeKnowApi.Data;
using LetMeKnowApi.ViewModels;

/*
@todo: Cambiar este controlador a TestController
solo verifica el autenticado o autorizacion
*/

namespace LetMeKnowApi.Controllers
{
    [Route("api/[controller]")]
    //[Authorize]
    public class TestsController : Controller
    {
        private readonly LetMeKnowContext _context;
        private readonly JsonSerializerSettings _serializerSettings;

        public TestsController(LetMeKnowContext context)
        {
            _context = context;    
            _serializerSettings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented
            };
        }

        /*[HttpPost]
        [AllowAnonymous]
        public IActionResult Get([FromForm] LoginViewModel user)
        {
            var existingUser = _context.Users.FirstOrDefault(u => u.UserName == user.UserName);

            if (existingUser == null)
            {
                return BadRequest("Invalid credentials");
            }else{
                // The JWT "sub" claim is automatically mapped to ClaimTypes.NameIdentifier
                // by the UseJwtBearerAuthentication middleware
                var username = HttpContext.User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;

                var response = new
                {
                    access_token = "All it's all rigth",
                    expires_in = "never"
                };

                var json = JsonConvert.SerializeObject(response, _serializerSettings);

                return new OkObjectResult(json);
            }
        }*/

         /*public string Get()
        {
            // The JWT "sub" claim is automatically mapped to ClaimTypes.NameIdentifier
            // by the UseJwtBearerAuthentication middleware
            var username = HttpContext.User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;

            return $"Hello {username}!";
        }*/


        [HttpGet]
        [Authorize(Policy = "DisneyUser")]
        public IActionResult Get()
        {
            var response = new
            {
                made_it = "Welcome Mickey!"
            };

            var json = JsonConvert.SerializeObject(response, _serializerSettings);
            return new OkObjectResult(json);
        }


        /*
        [AllowAnonymous]
        [Route("signin")]
        [HttpPost]
        public HttpResponseMessage Login(LoginViewModel model)
        {
            HttpResponseMessage response = null;
            if (ModelState.IsValid)
            {
                var existingUser = db.Users.FirstOrDefault(u => u.Email == model.Email);

                if (existingUser == null)
                {
                    response = Request.CreateResponse(HttpStatusCode.NotFound);
                }
                else
                {
                    var loginSuccess =
                        string.Equals(EncryptPassword(model.Password, existingUser.Salt),
                            existingUser.PasswordHash);

                    if (loginSuccess)
                    {
                        object dbUser;
                        var token = CreateToken(existingUser, out dbUser);
                        response = Request.CreateResponse(new {dbUser, token});
                    }
                }
            }
            else
            {
                response = Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }
            return response;
        }
         */

         /*
         [AllowAnonymous]
        [Route("signup")]
        [HttpPost]
        public HttpResponseMessage Register(RegisterViewModel model)
        {
            HttpResponseMessage response;
            if (ModelState.IsValid)
            {
                var existingUser = db.Users.FirstOrDefault(u => u.Email == model.Email);
                if (existingUser != null)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, "User already exist.");
                }

                //Create user and save to database
                var user = CreateUser(model);

                object dbUser;

                //Create token
                var token = CreateToken(user, out dbUser);

                response = Request.CreateResponse(new {dbUser, token});
            }
            else
            {
                response = Request.CreateResponse(HttpStatusCode.BadRequest, new {success = false});
            }

            return response;
        }
         */
    }
}
