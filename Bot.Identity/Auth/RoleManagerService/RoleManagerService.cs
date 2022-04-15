using System;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Core.Domain;
using Core.Dtos;
using Core.Identity;
using Core.Models.Contracts;
using Core.Models.Identity;
using Core.Options;
using System.Linq;
using System.Collections.Generic;

namespace Bot.Identity.Services
{
  public class RoleManagerService : IRoleManagerService
  {
    private readonly UserManager<AppUser> _userManager;
    private readonly IMapper _mapper;

    public RoleManagerService(
      UserManager<AppUser> userManager,
      IMapper mapper)
    {
      _mapper = mapper;
      _userManager = userManager;
    }

    public async Task<bool> ChangeUserRoles(string[] roles, string userId)
    {
      var user = await _userManager.FindByIdAsync(userId);
      var userRoles = await _userManager.GetRolesAsync(user);
      await _userManager.RemoveFromRolesAsync(user, userRoles);
      IEnumerable<string> enumRoles = roles;
      await _userManager.AddToRolesAsync(user, enumRoles);
      return true;
    }
  }
}