using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Infrastructure.Data;
using Core.Dtos;
using Infrastructure.Helpers;
using Core.Identity;
using Core.Models.Contracts;
using Infrastructure.Services;
using Bot.Identity.Services;
using Bot.Identity;
using Bot.Core.Responses;
using Bot.Identity.Extensions;

namespace WebAPI.Controllers
{

  [Authorize]
  public class AccountController : BaseApiController
  {
    private readonly UserManager<AppUser> _userManager;
    private readonly SignInManager<AppUser> _signInManager;
    private readonly ITokenService _tokenService;
    private readonly IMapper _mapper;
    private readonly IIdentityService _identityService;

    public AccountController(
      UserManager<AppUser> userManager,
      SignInManager<AppUser> signInManager,
      ITokenService tokenService,
      IMapper mapper,
      IIdentityService identityService)
    {
      _identityService = identityService;
      _signInManager = signInManager;
      _tokenService = tokenService;
      _mapper = mapper;
      _userManager = userManager;
    }

    [HttpGet]
    [Authorize]
    [Route("current")]
    public async Task<ActionResult<UserToReturnDto>> GetCurrentUser()
    {
      var user = await _userManager.FindByClaimsCurrentUser(HttpContext.User);

      if (user == null)
        return BadRequest("Пользователь не найден");

      var roles = await _userManager.GetRolesAsync(user);
      var userToReturn = _mapper.Map<AppUser, UserToReturnDto>(user);

      userToReturn.Token = await _tokenService.CreateToken(user);
      userToReturn.UserRoles = roles;

      return Ok(userToReturn);
    }

    [HttpGet]
    [Route("checkmail")]
    public async Task<ActionResult<bool>> CheckEmailExistsAsync([FromQuery] string email)
    {
      return await _userManager.FindByEmailAsync(email) != null;
    }



    [HttpPost]
    [AllowAnonymous]
    [Route("login")]
    public async Task<ActionResult<UserToReturnDto>> Login(UserToLoginDto loginDto)
    {
      var user = await _userManager.FindByEmailAsync(loginDto.Email);
      if (user == null) return Unauthorized(new ApiResponse(401));

      var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);
      if (!result.Succeeded) return Unauthorized(new ApiResponse(401));

      var roles = await _userManager.GetRolesAsync(user);

      return new UserToReturnDto
      {
        Email = user.Email,
        Token = await _tokenService.CreateToken(user),
        DisplayName = user.DisplayName,
        UserRoles = roles,
      };
    }

    [HttpPost]
    [AllowAnonymous]
    [Route("register")]
    public async Task<ActionResult<UserToReturnDto>> Register(UserToRegisterDto registerDto)
    {
      if (CheckEmailExistsAsync(registerDto.Email).Result.Value)
      {
        return new BadRequestObjectResult(new ApiValidationErrorResponse { Errors = new[] { "Пользователь уже существует" } });
      }

      var user = new AppUser
      {
        DisplayName = registerDto.DisplayName,
        Email = registerDto.Email,
        UserName = registerDto.Email,
      };

      var result = await _userManager.CreateAsync(user, registerDto.Password);
      if (!result.Succeeded) return BadRequest(new ApiResponse(400));

      return new UserToReturnDto
      {
        Email = user.Email,
        Token = await _tokenService.CreateToken(user),
        DisplayName = user.DisplayName,
      };

    }

    [Authorize]
    [HttpPut]
    [Route("update")]
    public async Task<ActionResult<AppUser>> UpdateProduct([FromBody] UserToReturnDto userDto)
    {
      var user = await _userManager.FindByClaimsCurrentUser(HttpContext.User);
      user.DisplayName = userDto.DisplayName;
      user.UserDescription = userDto.UserDescription;
      user.BankOfficeId = (int)userDto.BankOfficeId;

      var result = _userManager.UpdateAsync(user).Result;
      return Ok(result);

    }


    [AllowAnonymous]
    [HttpPost]
    [Route("auth/facebook")]
    public async Task<IActionResult> FacebookAuth(string accessToken)
    {
      var authResponse = await _identityService.LoginWithFacebookAsync(accessToken);

      if (!authResponse.Success)
      {
        return BadRequest(new AuthFailedResponse { Errors = authResponse.Errors });
      }

      return Ok(authResponse);

    }

    [AllowAnonymous]
    [HttpPost]
    [Route("google/auth")]
    public async Task<IActionResult> GoogleAuth(string accessToken)
    {
      var authResponse = await _identityService.LoginWithGoogleAsync(accessToken);

      if (!authResponse.Success)
      {
        return BadRequest(new AuthFailedResponse { Errors = authResponse.Errors });
      }

      return Ok(authResponse);

    }

    [AllowAnonymous]
    [HttpGet]
    [Route("facebook/tokenget")]
    public async Task<IActionResult> FacebookAuthToken(string code)
    {
      var response = await _identityService.GetFacebookAccessToken(code);

      if (String.IsNullOrEmpty(response.AccessToken))
        return BadRequest("Cannot get access token form facebook");

      return Ok(response);

    }

    [AllowAnonymous]
    [HttpGet]
    [Route("google/tokenget")]
    public async Task<IActionResult> GoogleAuthToken(string code)
    {
      var response = await _identityService.GetGoogleAccessToken(code);

      if (String.IsNullOrEmpty(response.AccessToken))
        return BadRequest("Cannot get access token from google");

      return Ok(response);

    }


    [AllowAnonymous]
    [HttpPut]
    [Route("onboarded")]
    public async Task<IActionResult> SetOnboardedStatus()
    {
      var user = await _userManager.FindByClaimsCurrentUser(HttpContext.User);
      if (user == null)
        return BadRequest("Пользователь не найден");

      var result = await _userManager.UpdateAsync(user);
      if (!result.Succeeded)
        return BadRequest("Произошла ошибка при обновлении пользователя");

      return Ok(true);

    }




  }
}