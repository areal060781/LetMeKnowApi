using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using LetMeKnowApi.Core;
using LetMeKnowApi.Data.Abstract;
using LetMeKnowApi.Model;
using LetMeKnowApi.ViewModels;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace LetMeKnowApi.Controllers
{
    [Route("api/[controller]")]
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

        [HttpGet("{id}", Name = "GetUser")]
        public IActionResult Get(int id)
        {
            User _user = _userRepository.GetSingle(u => u.Id == id, u => u.SuggestionsCreated);

            if (_user != null)
            {
                UserViewModel _userVM = Mapper.Map<User, UserViewModel>(_user);
                return new OkObjectResult(_userVM);
            }
            else
            {
                return NotFound();
            }
        }

        [HttpGet("{id}/suggestions", Name = "GetUserSuggestions")]
        public IActionResult GetSuggestions(int id)
        {
            IEnumerable<Suggestion> _userSuggestions = _suggestionRepository.FindBy(s => s.CreatorId == id);

            if (_userSuggestions != null)
            {
                IEnumerable<SuggestionViewModel> _userSuggestionsVM = Mapper.Map<IEnumerable<Suggestion>, IEnumerable<SuggestionViewModel>>(_userSuggestions);
                return new OkObjectResult(_userSuggestionsVM);
            }
            else
            {
                return NotFound();
            }
        }

        [HttpGet("{id}/details", Name = "GetUserDetails")]
        public IActionResult GetUserDetails(int id)
        {
            User _user = _userRepository.GetSingle(u => u.Id == id, u => u.SuggestionsCreated, u => u.Roles);

            if(_user != null)
            {
                UserDetailsViewModel _userDetailsVM = Mapper.Map<User, UserDetailsViewModel>(_user);
                
                foreach(var role in _user.Roles)
                {
                    Role _roleDb = _roleRepository.GetSingle(r => r.Id == role.RoleId, r => r.Users);
                    _userDetailsVM.Roles.Add(Mapper.Map<Role, RoleViewModel>(_roleDb));
                }
                return new OkObjectResult(_userDetailsVM);
            }
            else
            {
                return NotFound();
            }
        }

        [HttpPost]
        public IActionResult Create([FromBody]UserViewModel user)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            User _newUser = new User { UserName = user.UserName };

            _userRepository.Add(_newUser);
            _userRepository.Commit();

            user = Mapper.Map<User, UserViewModel>(_newUser);

            CreatedAtRouteResult result = CreatedAtRoute("GetUser", new { controller = "Users", id = user.Id }, user);
            return result;
        }

        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody]UserViewModel user)
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
            else
            {
                _userDb.UserName = user.UserName;
                //_userDb.Profession = user.Profession;
                //_userDb.Avatar = user.Avatar;
                _userRepository.Commit();
            }

            user = Mapper.Map<User, UserViewModel>(_userDb);

            return new NoContentResult();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            User _userDb = _userRepository.GetSingle(id);

            if (_userDb == null)
            {
                return new NotFoundResult();
            }
            else
            {
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
    
}
