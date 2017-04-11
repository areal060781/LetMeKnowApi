using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Security.Claims;

namespace LetMeKnowApi.Controllers
{
    [Route("api/[controller]")]
    //[Authorize]
    public class TestsController : Controller
    {        
        private readonly JsonSerializerSettings _serializerSettings;

        public TestsController()
        {              
            _serializerSettings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented
            };
        }        

        [HttpGet]        
        [Authorize(Roles = "Administrator,Sender")]
        //[Authorize(Policy = "AdministratorRole")]
        public IActionResult Get()
        {
            var username = HttpContext.User.Claims.First(c => c.Type == ClaimTypes.Name).Value;
            var userId = HttpContext.User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;
            var role = HttpContext.User.Claims.First(c => c.Type == ClaimTypes.Role).Value;

            var response = new
            {
                made_it = $"Hello {username}! Your Id is {userId} and you are enroll as {role}."
            };

            var json = JsonConvert.SerializeObject(response, _serializerSettings);
            return new OkObjectResult(json);
        }
    }
}
