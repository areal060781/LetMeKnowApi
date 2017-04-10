using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using LetMeKnowApi.Core;
using LetMeKnowApi.Data.Abstract;
using LetMeKnowApi.Model;
using LetMeKnowApi.ViewModels;
using Microsoft.AspNetCore.Authorization;

namespace LetMeKnowApi.Controllers
{
    [Route("api/[controller]")]
    [AllowAnonymous]
    public class UsersController : Controller
    {
        private IUserRepository _userRepository;    
        private ISuggestionRepository _suggestionRepository;
        private IUserRoleRepository _userRoleRepository;    
        private IRoleRepository _roleRepository;        

        int page = 1;
        int pageSize = 0;
        public UsersController(IUserRepository userRepository,
                                ISuggestionRepository suggestionRepository,
                                IUserRoleRepository userRoleRepository,
                                IRoleRepository roleRepository)
        {
            _userRepository = userRepository; 
            _suggestionRepository = suggestionRepository;   
            _userRoleRepository = userRoleRepository;
            _roleRepository = roleRepository;            
        }

        // GET api/users
        [HttpGet]        
        public IActionResult Get()
        {
            var pagination = Request.Headers["Pagination"];

            if (!string.IsNullOrEmpty(pagination))
            {
                string[] vals = pagination.ToString().Split(',');
                int.TryParse(vals[0], out page);
                int.TryParse(vals[1], out pageSize);
            }

            int currentPage = page;
            int currentPageSize = pageSize;
            var totalUsers = _userRepository.Count();

            if (pageSize == 0){
                pageSize = totalUsers;
                currentPageSize = pageSize;
            }

            var totalPages = (int)Math.Ceiling((double)totalUsers / pageSize);

            IEnumerable<User> _users = _userRepository
                .AllIncluding(u => u.SuggestionsCreated, u => u.Roles)
                .OrderBy(u => u.Id)
                .Skip((currentPage - 1) * currentPageSize)
                .Take(currentPageSize)
                .ToList();

            IEnumerable<UserViewModel> _usersVM = Mapper.Map<IEnumerable<User>, IEnumerable<UserViewModel>>(_users);

            Response.AddPagination(page, pageSize, totalUsers, totalPages);

            return new OkObjectResult(_usersVM);
        }

        // GET api/users/1
        [HttpGet("{id}", Name = "GetUser")]
        public IActionResult Get(int id)
        {
            User _user = _userRepository.GetSingle(u => u.Id == id, u => u.SuggestionsCreated);

            if (_user == null)
            {
                return NotFound();
            }
            
            UserViewModel _userVM = Mapper.Map<User, UserViewModel>(_user);
            return new OkObjectResult(_userVM);
        }

        // GET api/users/1/suggestions
        [HttpGet("{id}/suggestions", Name = "GetUserSuggestions")]
        public IActionResult GetSuggestions(int id)
        {
            IEnumerable<Suggestion> _userSuggestions = _suggestionRepository.FindBy(s => s.CreatorId == id);

            if (_userSuggestions == null)
            {
                return NotFound();
            }            

            IEnumerable<SuggestionViewModel> _userSuggestionsVM = Mapper.Map<IEnumerable<Suggestion>, IEnumerable<SuggestionViewModel>>(_userSuggestions);
            return new OkObjectResult(_userSuggestionsVM);
        }

        // GET api/users/1/details
        [HttpGet("{id}/details", Name = "GetUserDetails")]
        public IActionResult GetUserDetails(int id)
        {
            User _user = _userRepository.GetSingle(u => u.Id == id, u => u.SuggestionsCreated, u => u.Roles);

            if(_user == null)
            {
                return NotFound();
            }            

            UserDetailsViewModel _userDetailsVM = Mapper.Map<User, UserDetailsViewModel>(_user);                
            foreach(var role in _user.Roles)
            {
                Role _roleDb = _roleRepository.GetSingle(r => r.Id == role.RoleId, r => r.Users);
                _userDetailsVM.Roles.Add(Mapper.Map<Role, RoleViewModel>(_roleDb));
            }

            return new OkObjectResult(_userDetailsVM);
        }
           
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

        private bool EmailExists(string email, int? id)
        {        
            if (id != null){
                return (_userRepository.GetSingle(u => u.Email == email, u => u.Id != id) != null) ? true : false;
            }
            else
            {                
                return (_userRepository.GetSingle(u => u.Email == email) != null) ? true : false;
            }                        
        }

        // POST api/users 
        [HttpPost]
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

        // PUT api/users/1
        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody]UpdateUserViewModel user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            
            /*if (EmailExists(user.Email, id))
            {
                var message = new[] {"El email ha sido tomado por otro usuario"};
                var response = new { userName = message };                    
                return BadRequest(response);                    
            }*/

            User _userDb = _userRepository.GetSingle(id);

            if (_userDb == null)
            {
                return NotFound();
            }
            
            _userDb.Email = user.Email;                
            _userRepository.Commit();

            UserViewModel _userVM = Mapper.Map<User, UserViewModel>(_userDb);            
            
            //CreatedAtRouteResult result = CreatedAtRoute("GetUser", new { controller = "Users", id = _userDb.Id }, _userVM);

            return new NoContentResult();
        }

        // PUT api/users/1
        [HttpPut("{id}/changePassword", Name = "ChangePassword")]
        public IActionResult ChangePassword(int id, [FromBody]ChangePasswordViewModel user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }                      

            User _userDb = _userRepository.GetSingle(id);

            if (_userDb == null)
            {
                return NotFound();
            }
            
            //Verificar que la contraseña anterior y la de la bd existan.
            string oldPasswordHashed = Extensions.EncryptPassword(user.OldPassword, _userDb.Salt);
            if (_userDb.PasswordHash != oldPasswordHashed){
                var message = new[] {"La antigua contraseña no coincide"};
                var response = new { oldPassWord = message };                    
                return BadRequest(response);                     
            }

            string salt = Extensions.CreateSalt();
            string newPasswordHashed = Extensions.EncryptPassword(user.NewPassword, salt);
            _userDb.Salt = salt;
            _userDb.PasswordHash= newPasswordHashed;            
            _userRepository.Commit();                       

            UserViewModel _userVM = Mapper.Map<User, UserViewModel>(_userDb);            
            
            //CreatedAtRouteResult result = CreatedAtRoute("GetUser", new { controller = "Users", id = _userDb.Id }, _userVM);

            return new NoContentResult();
        }

        // DELETE api/users/1
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            User _userDb = _userRepository.GetSingle(id);

            if (_userDb == null)
            {
                return new NotFoundResult();
            }
            
            IEnumerable<Suggestion> _suggestions = _suggestionRepository.FindBy(a => a.CreatorId == id);
            IEnumerable<UserRole> _userRoles = _userRoleRepository.FindBy(s => s.UserId == id);

            foreach (var suggestion in _suggestions)
            {
                 _suggestionRepository.Delete(suggestion);
            }

            foreach (var userRole in _userRoles)
            {                    
                _userRoleRepository.Delete(userRole);
            }

            _userRepository.Delete(_userDb);
            _userRepository.Commit();

            return new NoContentResult();
        }
    }   
}