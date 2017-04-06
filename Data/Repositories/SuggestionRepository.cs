using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LetMeKnowApi.Data;
using LetMeKnowApi.Data.Abstract;
using LetMeKnowApi.Data.Repositories;
using LetMeKnowApi.Model;

namespace LetMeKnowApi.Data.Repositories
{
    public class SuggestionRepository : EntityBaseRepository<Suggestion>, ISuggestionRepository
    {
        public SuggestionRepository(LetMeKnowContext context)
            : base(context)
        { }
    }
}
