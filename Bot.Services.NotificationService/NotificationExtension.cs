using System.Linq;
using Bot.Core.Responses;
using EventService;
using EventService.Event;
using Hangfire;
using Infrastructure.Services.TelegramService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NotificationService.JobManagment;

namespace NotificationService.Extensions
{
  public static class NotificationExtension
  {
    public static IServiceCollection AddNotificationExtension(this IServiceCollection services, IConfiguration _config)
    {
      services.AddHangfire(h =>
        h.UseSqlServerStorage(_config.GetConnectionString("DefaultConnection")));
      services.AddHangfireServer();

      services.AddScoped<ITelegramService, TelegramService>();
      services.AddScoped<IEventManager, EventManager>();
      services.AddScoped<IJobManager, JobManager>();




      services.Configure<ApiBehaviorOptions>(options =>
      {
        options.InvalidModelStateResponseFactory = actionContext =>
        {
          var errors = actionContext.ModelState
            .Where(e => e.Value.Errors.Count > 0)
            .SelectMany(x => x.Value.Errors)
            .Select(x => x.ErrorMessage).ToArray();

          var errorResponse = new ApiValidationErrorResponse()
          {
            Errors = errors
          };

          return new BadRequestObjectResult(errorResponse);

        };

      });

      return services;

    }
  }
}





