using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using LetMeKnowApi.Core;
using LetMeKnowApi.Data.Abstract;
using LetMeKnowApi.Model;
using LetMeKnowApi.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LetMeKnowApi.Controllers
{
    [Route("api/[controller]")]
    [AllowAnonymous]
    public class RolesController : Controller
    {
        public IRoleRepository _roleRepository;        
        public IUserRepository _userRepository;
        public IUserRoleRepository _userRoleRepository;
        public ISuggestionRepository _suggestionRepository;
        int page = 1;
        int pageSize = 0;
        public RolesController(IRoleRepository roleRepository, IUserRepository userRepository, IUserRoleRepository userRoleRepository, ISuggestionRepository suggestionRepository)
        {
            _roleRepository = roleRepository;
            _userRepository = userRepository;
            _userRoleRepository = userRoleRepository;
            _suggestionRepository = suggestionRepository;
        }

        // GET api/roles
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
            var totalRoles = _roleRepository.Count();

            if (pageSize == 0){
                pageSize = totalRoles;
                currentPageSize = pageSize;
            } 

            var totalPages = (int)Math.Ceiling((double)totalRoles / pageSize);

            IEnumerable<Role> _roles = _roleRepository
                .AllIncluding(a => a.Users)
                .OrderBy(a => a.Id)
                .Skip((currentPage - 1) * currentPageSize)
                .Take(currentPageSize)
                .ToList();
            
            IEnumerable<RoleViewModel> _rolesVM = Mapper.Map<IEnumerable<Role>, IEnumerable<RoleViewModel>>(_roles);

            Response.AddPagination(page, pageSize, totalRoles, totalPages);

            return new OkObjectResult(_rolesVM);        
        }

        // GET api/roles/5
        [HttpGet("{id}", Name="GetRole")]
        public IActionResult Get(int id)
        {
            Role _role = _roleRepository.GetSingle(a => a.Id == id, a => a.Users);

            if (_role == null)            
            {
                return NotFound();
            }

            RoleViewModel _roleVM = Mapper.Map<Role, RoleViewModel>(_role);

            return new OkObjectResult(_roleVM);
        }

        // GET api/roles/5/details
        [HttpGet("{id}/details", Name="GetRoleDetails")]
        public IActionResult GetRoleDetails(int id)
        {
            Role _role = _roleRepository.GetSingle(r => r.Id == id, r => r.Users);

            if (_role == null)
            {
                return NotFound();
            }

            RoleDetailsViewModel _roleDetailsVM = Mapper.Map<Role, RoleDetailsViewModel>(_role);
            foreach (var user in _role.Users)
            {
                User _userDb = _userRepository.GetSingle(user.UserId);            
                _roleDetailsVM.Users.Add(Mapper.Map<User, UserViewModel>(_userDb));
            }

            return new OkObjectResult(_roleDetailsVM);            
        }

        // POST api/roles
        [HttpPost]
        public IActionResult Create([FromBody]RoleViewModel role)    
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Role _newRole = new Role { Name = role.Name };
            _roleRepository.Add(_newRole);
            _roleRepository.Commit();

            role = Mapper.Map<Role, RoleViewModel>(_newRole);

            CreatedAtRouteResult result = CreatedAtRoute("GetRole", new { controller = "Roles", id = role.Id }, role);
            return result;
        }

        // PUT api/roles/5
        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody]RoleViewModel role)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Role _roleDb = _roleRepository.GetSingle(id);

            if (_roleDb == null)
            {
                return NotFound();
            }
            
            _roleDb.Name = role.Name;
            _roleRepository.Commit();            
            role = Mapper.Map<Role, RoleViewModel>(_roleDb);

            return new NoContentResult();
        }

        // DELETE api/roles/5
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            Role _roleDb = _roleRepository.GetSingle(id);

            if(_roleDb == null)
            {
                return new NotFoundResult();
            }
            
            IEnumerable<UserRole> _userRoles = _userRoleRepository.FindBy(s => s.RoleId == id);
            foreach(var userRole in _userRoles)
            {
                IEnumerable<Suggestion> _suggestions = _suggestionRepository.FindBy(s => s.CreatorId == userRole.UserId);
                foreach(var suggestion in _suggestions)
                {
                    _suggestionRepository.Delete(suggestion);
                }
                _userRepository.DeleteWhere(u => u.Id == userRole.UserId);
                _userRoleRepository.Delete(userRole);
            }                   

            _roleRepository.Delete(_roleDb);
            _roleRepository.Commit();

            return new NoContentResult(); 
        }
    }
}    