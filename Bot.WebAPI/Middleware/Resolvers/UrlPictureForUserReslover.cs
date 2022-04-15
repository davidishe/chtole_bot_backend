using AutoMapper;
using Core.Dtos;
using Core.Identity;
using Core.Models;
using Microsoft.Extensions.Configuration;

namespace WebAPI.Helpers
{
  public class UrlPictureForUserReslover : IValueResolver<AppUser, UserToReturnDto, string>
  {
    public UrlPictureForUserReslover()
    {
    }

    private readonly IConfiguration _config;

    public UrlPictureForUserReslover(IConfiguration config)
    {
      _config = config;
    }

    public string Resolve(AppUser source, UserToReturnDto destination, string destMember, ResolutionContext context)
    {
      if (!string.IsNullOrEmpty(source.PictureUrl))
      {
        return _config.GetSection("ApiUrl").Value + "Assets/Images/Users/" + source.PictureUrl;
      }
      return null;
    }
  }

}