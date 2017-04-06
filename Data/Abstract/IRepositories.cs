using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LetMeKnowApi.Model;

namespace LetMeKnowApi.Data.Abstract
{
    public interface IUserRepository : IEntityBaseRepository<User> { }
    public interface IRoleRepository : IEntityBaseRepository<Role> { }
    public interface IUserRoleRepository : IEntityBaseRepository<UserRole> { }
    public interface IAreaRepository : IEntityBaseRepository<Area> { }
    public interface ISuggestionRepository : IEntityBaseRepository<Suggestion> { } 
}
