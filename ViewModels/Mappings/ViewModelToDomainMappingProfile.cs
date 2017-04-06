using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using LetMeKnowApi.Model;

namespace LetMeKnowApi.ViewModels.Mappings
{
    public class ViewModelToDomainMappingProfile : Profile
    {
        protected override void Configure()
        {
            Mapper.CreateMap<SuggestionViewModel, Suggestion>()
               .ForMember(s => s.Creator, map => map.UseValue(null))
               .ForMember(s => s.Area, map => map.UseValue(null));     

            Mapper.CreateMap<UpdateSuggestionViewModel, Suggestion>()               
               .ForMember(s => s.Area, map => map.UseValue(null));           

            Mapper.CreateMap<UserViewModel, User>();                

            Mapper.CreateMap<RoleViewModel, Role>();

            Mapper.CreateMap<AreaViewModel, Area>();
        }
    }
}
