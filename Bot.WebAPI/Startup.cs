using System.IO;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using WebAPI.Middleware;
using StackExchange.Redis;
using Infrastructure.Extensions;
using Microsoft.Extensions.Hosting;
using Hangfire;
using Microsoft.OpenApi.Models;
using NotificationService.Extensions;
using Bot.Infrastructure.Database;
using Bot.Services.Telegram.Extensions;
using Bot.Identity;
using Bot.Identity.Database.Extensions;
using Bot.Identity.Database;
using Core.Models;
using Bot.Infrastructure;
using Infrastructure.Services.TaskManagerService;

namespace WebAPI
{
  public class Startup
  {
    public Startup(IConfiguration configuration)
    {
      _config = configuration;
    }

    public IConfiguration _config { get; }


    public void ConfigureDevelopmentServices(IServiceCollection services)
    {
      ConfigureServices(services);
    }

    public void ConfigureProductionServices(IServiceCollection services)
    {
      ConfigureServices(services);
    }

    public void ConfigureServices(IServiceCollection services)
    {

      services.AddDbContext<AppDbContext>(options => options.UseSqlServer(_config.GetConnectionString("DefaultConnection")));
      services.AddDbContext<IdentityContext>(options => options.UseSqlServer(_config.GetConnectionString("IdentityConnection")));


      services.AddTelegramBotClient(_config);
      services.AddNotificationExtension(_config);


      services.AddSingleton<IConnectionMultiplexer>(c =>
      {
        var configuration = ConfigurationOptions.Parse(_config.GetConnectionString("Redis"), true);
        return ConnectionMultiplexer.Connect(configuration);
      });

      services.AddIdentityServices(_config);

      services.AddControllers(options =>
      {
        var policy = new AuthorizationPolicyBuilder()
          .RequireAuthenticatedUser()
          .Build();

        options.Filters.Add(new AuthorizeFilter(policy));
      })
        .AddNewtonsoftJson(opt =>
        {
          opt.SerializerSettings.ReferenceLoopHandling =
          Newtonsoft.Json.ReferenceLoopHandling.Ignore;
        });

      services.AddCors();
      services.AddAutoMapper(typeof(Startup));
      services.AddApplicationServices();
      // services.AddSingleton<IReviewerRepository, ReviewerRepository>();
      services.AddScoped<IDbRepository<Reviewer>, DbRepository<Reviewer>>();

      services.AddSwaggerGen(c =>
      {
        c.SwaggerDoc("v1", new OpenApiInfo { Title = "NotificationService", Version = "v1" });
      });


    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {

      if (env.IsDevelopment())
      {
        app.UseDeveloperExceptionPage();
        app.UseSwagger();
        app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Chtole bot v1"));
      }


      app.UseCors(x => x.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
      app.UseMiddleware<ExceptionMiddleware>();
      app.UseStatusCodePagesWithReExecute("/errors/{0}");
      app.UseRouting();
      app.UseAuthentication();
      app.UseAuthorization();
      app.UseHangfireDashboard("/dashboard");
      app.UseDefaultFiles();
      app.UseStaticFiles();
      app.UseStaticFiles(new StaticFileOptions
      {
        FileProvider = new PhysicalFileProvider(
          Path.Combine(Directory.GetCurrentDirectory(), "Assets")
        ),
        RequestPath = "/assets"
      });
      app.UseEndpoints(endpoints =>
      {
        endpoints.MapControllers();
        endpoints.MapFallbackToController("Index", "Fallback");
      });
    }
  }
}
