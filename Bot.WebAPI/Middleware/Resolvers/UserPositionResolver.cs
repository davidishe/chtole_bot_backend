// using System.Linq;
// using System.Security.Claims;
// using AutoMapper;
// using Core.Dtos;
// using Core.Identity;
// using Core.Models;
// using Infrastructure.Data.Contexts;
// using Microsoft.AspNetCore.Identity;
// using Microsoft.Extensions.Configuration;


// namespace WebAPI.Helpers
// {
//   public class UserPositionResolver : IValueResolver<Item, ItemToReturnDto, string>
//   {
//     public UserPositionResolver()
//     {
//     }

//     private readonly UserManager<AppUser> _userManager;
//     private readonly IdentityContext _identityContext;



//     public UserPositionResolver(
//       UserManager<AppUser> userManager,
//       IdentityContext identityContext
//     )
//     {
//       _userManager = userManager;
//       _identityContext = identityContext;
//     }

//     public string Resolve(Item source, ItemToReturnDto destination, string destMember, ResolutionContext context)
//     {
//       var userPositionId = _userManager.FindByIdAsync(source.UserId.ToString()).Result.UserPositionId;
//       var position = _identityContext.UserPosition.Where(x => x.Id == userPositionId).FirstOrDefault().Name;
//       return position;
//     }
//   }

// }