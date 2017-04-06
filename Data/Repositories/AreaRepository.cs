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
    public class AreaRepository : EntityBaseRepository<Area>, IAreaRepository
    {
        public AreaRepository(LetMeKnowContext context)
            : base(context)
        { }
    }
}
