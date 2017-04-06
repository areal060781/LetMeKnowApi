using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using LetMeKnowApi.Data.Abstract;
using LetMeKnowApi.Model;
using LetMeKnowApi.ViewModels;
using LetMeKnowApi.Core;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace LetMeKnowApi.Controllers
{
    [Route("api/[controller]")]
    public class SuggestionsController : Controller
    {
        private ISuggestionRepository _suggestionRepository;
        int page = 1;
        int pageSize = 0;
        public SuggestionsController(ISuggestionRepository suggestionRepository)
        {
            _suggestionRepository = suggestionRepository;
        }
        // GET api/suggestions
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
            var totalSuggestions = _suggestionRepository.Count();            

            if (pageSize == 0){
                pageSize = totalSuggestions;
                currentPageSize = pageSize;
            }

            var totalPages = (int)Math.Ceiling((double)totalSuggestions / pageSize);

            IEnumerable<Suggestion> _suggestions = _suggestionRepository
                .AllIncluding(s => s.Creator, s => s.Area)                                
                .OrderByDescending(s => s.Id)
                .Skip((currentPage - 1) * currentPageSize)
                .Take(currentPageSize)
                .ToList();

            IEnumerable<SuggestionViewModel> _suggestionsVM = Mapper.Map<IEnumerable<Suggestion>, IEnumerable<SuggestionViewModel>>(_suggestions);

            Response.AddPagination(page, pageSize, totalSuggestions, totalPages);

            return new OkObjectResult(_suggestionsVM);

        }

        // GET api/suggestions/5
        [HttpGet("{id}", Name = "GetSuggestion")]
        public IActionResult Get(int id)
        {
            Suggestion _suggestion = _suggestionRepository
                .GetSingle(s => s.Id == id, s => s.Creator, s => s.Area);

            if (_suggestion != null)
            {
                SuggestionViewModel _suggestionVM = Mapper.Map<Suggestion, SuggestionViewModel>(_suggestion);
                return new OkObjectResult(_suggestionVM);
            }
            else
            {
                return NotFound();
            }
        }

        // POST api/suggestions
        [HttpPost]
        public IActionResult Create([FromBody]SuggestionViewModel suggestion)        
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Suggestion _newSuggestion = Mapper.Map<SuggestionViewModel, Suggestion>(suggestion);
            _newSuggestion.DateCreated = DateTime.Now;
            _newSuggestion.DateUpdated = _newSuggestion.DateCreated;
            _newSuggestion.Status = SuggestionStatus.Nuevo;

            _suggestionRepository.Add(_newSuggestion);
            _suggestionRepository.Commit();

            suggestion = Mapper.Map<Suggestion, SuggestionViewModel>(_newSuggestion);

            CreatedAtRouteResult result = CreatedAtRoute("GetSuggestion", new { controller = "Suggestions", id = suggestion.Id }, suggestion);

            return result;
        }

        // PUT api/suggestions/5
        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody]UpdateSuggestionViewModel suggestion)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Suggestion _suggestionDB = _suggestionRepository.GetSingle(id);

            if(_suggestionDB == null)
            {
                return NotFound();
            }
            else{
                _suggestionDB.Title = suggestion.Title;
                _suggestionDB.Content = suggestion.Content;
                _suggestionDB.Image = suggestion.Image;
                _suggestionDB.AreaId = suggestion.AreaId;
                _suggestionDB.DateUpdated = DateTime.Now;

                //Only if user is admin:                
                //_suggestionDB.Status = (SuggestionStatus)Enum.Parse(typeof(SuggestionStatus), suggestion.Status);

                _suggestionRepository.Commit();
            }

            suggestion = Mapper.Map<Suggestion, UpdateSuggestionViewModel>(_suggestionDB);

            return new NoContentResult();
        }

        // DELETE api/suggestions/5
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            Suggestion _suggestionDB = _suggestionRepository.GetSingle(id);

            if (_suggestionDB == null)
            {
                return new NotFoundResult();
            }
            else
            {
                _suggestionRepository.Delete(_suggestionDB);
                _suggestionRepository.Commit();
                return new NoContentResult();

            }

        }
    }
}
