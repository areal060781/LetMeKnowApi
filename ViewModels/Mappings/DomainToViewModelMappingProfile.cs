using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using LetMeKnowApi.Model;

namespace LetMeKnowApi.ViewModels.Mappings
{
    public class DomainToViewModelMappingProfile : Profile
    {
        protected override void Configure()
        {
            Mapper.CreateMap<User, UserViewModel>()
                .ForMember(vm => vm.SuggestionsCreated,
                    map => map.MapFrom(u => u.SuggestionsCreated.Count()))
                .ForMember(vm => vm.Roles, 
                    map => map.MapFrom(s => s.Roles.Count()));

            Mapper.CreateMap<User, UserDetailsViewModel>()
                .ForMember(vm => vm.SuggestionsCreated,
                    map => map.MapFrom(u => u.SuggestionsCreated.Count()))
                .ForMember(vm => vm.Roles, 
                    map => map.UseValue(new List<RoleViewModel>()));

            Mapper.CreateMap<Role, RoleViewModel>()
                .ForMember(vm => vm.Users,
                    map => map.MapFrom(u => u.Users.Count()));

            Mapper.CreateMap<Role, RoleDetailsViewModel>()
                .ForMember(vm => vm.Users,
                    map => map.UseValue(new List<UserViewModel>()));

            Mapper.CreateMap<Area, AreaViewModel>()
                .ForMember(vm => vm.Suggestions,
                    map => map.MapFrom(u => u.Suggestions.Count()));
                    
            Mapper.CreateMap<Suggestion, SuggestionViewModel>()
               .ForMember(vm => vm.Creator,
                    map => map.MapFrom(s => s.Creator.UserName))
               .ForMember(vm => vm.Area,
                    map => map.MapFrom(s => s.Area.Name));

            Mapper.CreateMap<Suggestion, UpdateSuggestionViewModel>()               
               .ForMember(vm => vm.Area,
                    map => map.MapFrom(s => s.Area.Name));                        
        }
    }
}
