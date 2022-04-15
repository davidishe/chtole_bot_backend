using System;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Core.Domain;
using Core.Dtos;
using Core.Identity;
using Core.Models.Contracts;

namespace Bot.Identity.Services
{
  public class IdentityService : IIdentityService
  {
    private readonly UserManager<AppUser> _userManager;
    private readonly IFacebookAuthService _facebookAuthService;
    private readonly IGoogleAuthService _googleAuthService;
    private readonly ITokenService _tokenService;
    private readonly IMapper _mapper;

    public IdentityService(
      UserManager<AppUser> userManager,
      IFacebookAuthService facebookAuthService,
      IGoogleAuthService googleAuthService,
      ITokenService tokenService,
      IMapper mapper)
    {
      _mapper = mapper;
      _userManager = userManager;
      _facebookAuthService = facebookAuthService;
      _googleAuthService = googleAuthService;
      _tokenService = tokenService;
    }


    #region facebook login logic

    public async Task<AccesssTokenResponse> GetFacebookAccessToken(string code)
    {
      var acessToken = await _facebookAuthService.GetAccessTokenAsync(code);
      return acessToken;
    }

    public async Task<AccesssTokenResponse> GetGoogleAccessToken(string code)
    {
      var acessToken = await _googleAuthService.GetAccessTokenAsync(code);
      return acessToken;
    }


    public async Task<ExternalAuthResult> LoginWithFacebookAsync(string accessToken)
    {
      var validatedTokenResult = await _facebookAuthService.ValidateAccessTokenAsync(accessToken);

      if (!validatedTokenResult.Data.IsValid)
      {
        return new ExternalAuthResult { Errors = new[] { "Invalid Facebook token" } };
      }

      var userInfo = await _facebookAuthService.GetUserInfoAsync(accessToken);
      var user = await _userManager.FindByEmailAsync(userInfo.Email);


      if (user == null)
      {
        var newUser = new AppUser
        {
          Email = userInfo.Email,
          UserName = userInfo.Email,
          DisplayName = userInfo.FirstName,
          PictureUrl = userInfo.FacebookPicture.Data.Url.ToString()
        };

        var result = await _userManager.CreateAsync(newUser);
        if (!result.Succeeded)
        {
          return new ExternalAuthResult { Errors = new[] { "Invalid Facebook auth" } };
        }

        var userToReturn = await _userManager.FindByEmailAsync(newUser.Email);

        return new ExternalAuthResult
        {
          Token = await _tokenService.CreateToken(userToReturn),
          Success = true,
          User = _mapper.Map<AppUser, UserToReturnDto>(newUser)
        };
      }

      return new ExternalAuthResult
      {
        Token = await _tokenService.CreateToken(user),
        Success = true,
        User = _mapper.Map<AppUser, UserToReturnDto>(user)
      };

    }
    #endregion


    public async Task<ExternalAuthResult> LoginWithGoogleAsync(string accessToken)
    {
      var validatedTokenResult = await _googleAuthService.ValidateAccessTokenAsync(accessToken);

      if (String.IsNullOrEmpty(validatedTokenResult.ClientId))
      {
        return new ExternalAuthResult { Errors = new[] { "Invalid Google token" } };
      }

      var userInfo = await _googleAuthService.GetUserInfoAsync(accessToken);
      var user = await _userManager.FindByEmailAsync(userInfo.Email);


      if (user == null)
      {
        var newUser = new AppUser
        {
          Email = userInfo.Email,
          UserName = userInfo.Email,
          DisplayName = userInfo.FirstName,
          PictureUrl = userInfo.PictureUrl.ToString()
        };

        var result = await _userManager.CreateAsync(newUser);
        if (!result.Succeeded)
        {
          return new ExternalAuthResult { Errors = new[] { "Something went wrong" } };
        }

        var userToReturn = await _userManager.FindByEmailAsync(newUser.Email);

        return new ExternalAuthResult
        {
          Token = await _tokenService.CreateToken(userToReturn),
          Success = true,
          User = _mapper.Map<AppUser, UserToReturnDto>(newUser)
        };
      }

      return new ExternalAuthResult
      {
        Token = await _tokenService.CreateToken(user),
        Success = true,
        User = _mapper.Map<AppUser, UserToReturnDto>(user)
      };

    }





  }
}