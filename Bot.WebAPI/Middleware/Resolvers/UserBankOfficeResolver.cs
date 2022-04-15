using System.Linq;
using System.Security.Claims;
using AutoMapper;
using Bot.Infrastructure.Specifications;
using Core.Dtos;
using Core.Identity;
using Core.Models;
using Infrastructure.Database;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;


namespace WebAPI.Helpers
{

  public class UserBankOfficeResolver : IValueResolver<AppUser, UserToReturnDto, string>
  {
    public UserBankOfficeResolver()
    {
    }

    private readonly UserManager<AppUser> _userManager;
    private readonly IGenericRepository<Office> _officeRepo;


    public UserBankOfficeResolver(
      UserManager<AppUser> userManager,
      IGenericRepository<Office> officeRepo
    )
    {
      _userManager = userManager;
      _officeRepo = officeRepo;
    }

    public string Resolve(AppUser source, UserToReturnDto destination, string destMember, ResolutionContext officeRepo)
    {
      var spec = new BaseSpecification<Office>();
      var officeName = _officeRepo.ListAsync(spec).Result.Where(x => x.Id == source.BankOfficeId).FirstOrDefault().Name;
      return officeName;
    }
  }

}