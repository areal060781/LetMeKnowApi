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
    public class AreasController : Controller
    {
        public IAreaRepository _areaRepository;
        public ISuggestionRepository _suggestionRepository;
        int page = 1;
        int pageSize = 0;
        public AreasController(IAreaRepository areaRepository, ISuggestionRepository suggestionRepository)
        {
            _areaRepository = areaRepository;
            _suggestionRepository = suggestionRepository;
        }

        // GET api/areas
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
            var totalAreas = _areaRepository.Count();   

            if (pageSize == 0){
                pageSize = totalAreas;
                currentPageSize = pageSize;
            }                  

            var totalPages = (int)Math.Ceiling((double)totalAreas / pageSize);

            IEnumerable<Area> _areas = _areaRepository
                .AllIncluding(a => a.Suggestions)
                .OrderBy(a => a.Id)
                .Skip((currentPage - 1) * currentPageSize)
                .Take(currentPageSize)
                .ToList();
            
            IEnumerable<AreaViewModel> _areasVM = Mapper.Map<IEnumerable<Area>, IEnumerable<AreaViewModel>>(_areas);

            Response.AddPagination(page, pageSize, totalAreas, totalPages);

            return new OkObjectResult(_areasVM);
            
        }

        // GET api/areas/5
        [HttpGet("{id}", Name="GetArea")]
        public IActionResult Get(int id)
        {
            Area _area = _areaRepository.GetSingle(a => a.Id == id, a => a.Suggestions);

            if (_area == null)
            {
                return NotFound();
            }
            
            AreaViewModel _areaVM = Mapper.Map<Area, AreaViewModel>(_area);
            return new OkObjectResult(_areaVM);
        }

        // GET api/areas/5/suggestions
        [HttpGet("{id}/suggestions", Name="GetAreaSuggestions")]
        public IActionResult GetSuggestions(int id)
        {
            IEnumerable<Suggestion> _areaSuggestions = _suggestionRepository.FindBy(s => s.AreaId == id);

            if (_areaSuggestions == null)
            {
                return NotFound();
            }
            
            IEnumerable<SuggestionViewModel> _areaSuggestionsVM = Mapper.Map<IEnumerable<Suggestion>, IEnumerable<SuggestionViewModel>>(_areaSuggestions);
            return new OkObjectResult(_areaSuggestionsVM);
        }

        // POST api/areas
        [HttpPost]
        public IActionResult Create([FromBody]AreaViewModel area)    
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Area _newArea = new Area { Name = area.Name };
            _areaRepository.Add(_newArea);
            _areaRepository.Commit();

            area = Mapper.Map<Area, AreaViewModel>(_newArea);

            CreatedAtRouteResult result = CreatedAtRoute("GetArea", new { controller = "Areas", id = area.Id }, area);
            return result;
        }

        // PUT api/areas/5
        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody]AreaViewModel area)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Area _areaDb = _areaRepository.GetSingle(id);

            if (_areaDb == null)
            {
                return NotFound();
            }
            
            _areaDb.Name = area.Name;
            _areaRepository.Commit();
            area = Mapper.Map<Area, AreaViewModel>(_areaDb);

            return new NoContentResult();
        }

        // DELETE api/areas/5
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            Area _areaDb = _areaRepository.GetSingle(id);

            if(_areaDb == null)
            {
                return new NotFoundResult();
            }            

            IEnumerable<Suggestion> _suggestions = _suggestionRepository.FindBy(s => s.AreaId == id);
            foreach(var suggestion in _suggestions)
            {
                _suggestionRepository.Delete(suggestion);
            }    

             _areaRepository.Delete(_areaDb);
             _areaRepository.Commit();

            return new NoContentResult();    
        }
    }
}    