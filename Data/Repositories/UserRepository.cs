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
    public class UserRepository : EntityBaseRepository<User>, IUserRepository
    {
        public UserRepository(LetMeKnowContext context)
            : base(context)
        { }
    }
}
