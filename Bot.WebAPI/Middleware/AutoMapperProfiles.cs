using AutoMapper;
using Core.Dtos;
using Core.Identity;
using Core.Models;

namespace WebAPI.Helpers
{
  public class AutoMapperProfiles : Profile
  {

    public AutoMapperProfiles()
    {
      CreateMap<AppUser, UserToReturnDto>()
        .ForMember(d => d.PictureUrl, m => m.MapFrom(s => s.PictureUrl))
        .ForMember(d => d.PictureUrl, m => m.MapFrom<UrlPictureForUserReslover>())
        .ForMember(d => d.BankOffice, m => m.MapFrom<UserBankOfficeResolver>())
        .ForMember(d => d.UserRoles, m => m.MapFrom<UserRolesReslover>());

      CreateMap<Item, ItemDto>()
        .ForMember(d => d.AppUser, m => m.MapFrom<UserResolver>())
        .ForMember(d => d.ItemType, m => m.MapFrom(s => s.ItemType.Name));

      CreateMap<Member, MemberDto>();


      CreateMap<Member, MemberItemDto>()
        .ForMember(d => d.Name, m => m.MapFrom(s => s.Name))
        .ForMember(d => d.IsEnabled, m => m.MapFrom(s => s.IsEnabled));


    }



  }
}