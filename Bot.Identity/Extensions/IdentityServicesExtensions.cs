using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Core.Identity;
using Core.Models.Identity;
using Core.Options;
using Bot.Identity.Services;

namespace Bot.Identity.Database.Extensions
{
  public static class IdentityServicesExtensions

  {
    public static IServiceCollection AddIdentityServices(this IServiceCollection services, IConfiguration config)
    {

      IdentityBuilder builder = services.AddIdentityCore<AppUser>(opt =>
      {
        opt.Password.RequireDigit = false;
        opt.Password.RequiredLength = 4;
        opt.Password.RequireNonAlphanumeric = false;
        opt.Password.RequireUppercase = false;
        opt.Password.RequireLowercase = false;
      });

      var b = new IdentityBuilder(builder.UserType, typeof(Role), builder.Services);
      b.AddEntityFrameworkStores<IdentityContext>();
      b.AddRoleValidator<RoleValidator<Role>>();
      b.AddRoleManager<RoleManager<Role>>();
      b.AddSignInManager<SignInManager<AppUser>>();


      // jwt id service
      var jwtSettings = new JwtSettings();
      config.Bind(nameof(jwtSettings), jwtSettings);
      services.AddSingleton(jwtSettings);

      services.AddScoped<ITokenService, TokenService>();
      services.AddScoped<IIdentityService, IdentityService>();
      services.AddScoped<IRoleManagerService, RoleManagerService>();
      services.AddScoped<IUserPositionsService, UserPositionsService>();


      // facebook settings
      var facebookAuthSettings = new FacebookAuthSettings();
      config.Bind(nameof(FacebookAuthSettings), facebookAuthSettings);
      services.AddSingleton(facebookAuthSettings);
      services.AddSingleton<IFacebookAuthService, FacebookAuthService>();
      services.AddHttpClient();

      services.AddSingleton<IGoogleAuthService, GoogleAuthService>();



      var tokenValidationParameters = new TokenValidationParameters
      {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtSettings.Secret)),
        ValidateIssuer = false,
        ValidateAudience = false,
        RequireExpirationTime = false,
        ValidateLifetime = true
      };
      services.AddSingleton(tokenValidationParameters);

      services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer
        (options =>
        {
          options.TokenValidationParameters = new TokenValidationParameters
          {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Token:Key"])),
            ValidIssuer = config["Token:Issuer"],
            ValidateIssuer = true,
            ValidateAudience = false
          };
        });

      // .AddFacebook(options =>
      // {
      //   options.AppId = config["Authentication:Facebook:FacebookAppId"];
      //   options.AppSecret = config["Authentication:Facebook:FacebokAppSecret"];
      // })


      services.AddAuthorization(options =>
      {
        options.AddPolicy("RequireAdminRole", policy => policy.RequireRole("Admin"));
        options.AddPolicy("RequireModerator", policy => policy.RequireRole("Admin, Moderator"));
        options.AddPolicy("RequireAuthentication", policy => policy.RequireRole("Admin, Moderator, Client"));

      });

      return services;
    }

  }
}