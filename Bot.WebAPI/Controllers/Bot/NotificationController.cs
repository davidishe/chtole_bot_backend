using Microsoft.AspNetCore.Mvc;
using System;
using Hangfire;
using WebAPI.Controllers;
using Core.Dtos;
using Microsoft.AspNetCore.Authorization;
using EventService;

namespace WebAPI.Controllers
{
  public class NotificationController : BaseApiController
  {
    public IEventManager _eventManager;
    public NotificationController(
      IEventManager eventManager
    )
    {
      _eventManager = eventManager;
    }

    [HttpPost]
    [AllowAnonymous]
    [Route("fire-and-forget")]
    public IActionResult FireAndForget(string client)
    {
      string jobId = BackgroundJob.Enqueue(() =>
          Console.WriteLine($"{client}, thank you for contacting us."));

      return Ok($"Job Id: {jobId}");
    }

    [HttpPost]
    [Route("delayed")]
    [AllowAnonymous]
    public IActionResult Delayed(string client)
    {
      string jobId = BackgroundJob.Schedule(() =>
          Console.WriteLine($"Session for client {client} has been closed."), TimeSpan.FromSeconds(60));

      return Ok($"Job Id: {jobId}");
    }

    [HttpPost]
    [Route("recurring/add")]
    [Obsolete]
    [AllowAnonymous]
    public IActionResult Recurring()
    {
      var timeZone = TimeZone.CurrentTimeZone;
      var jobId = Guid.NewGuid().ToString();

      RecurringJob.AddOrUpdate(jobId, () => _eventManager.ExecuteRegularEvent(jobId), "* 56 20 * * ?", TimeZoneInfo.Local);
      return Ok(jobId);
    }

    [HttpPost]
    [Route("recurring/update")]
    [AllowAnonymous]
    public IActionResult Recurring(JobForUpdateDto jobForUpdateDto)
    {
      var timeZone = TimeZoneInfo.GetSystemTimeZones();
      RecurringJob.AddOrUpdate(jobForUpdateDto.JobId, () => _eventManager.ExecuteRegularEvent(jobForUpdateDto.JobId), jobForUpdateDto.CronExpression, TimeZoneInfo.Local);
      return Ok(200);
    }

    [HttpPost]
    [Route("continuations")]
    public IActionResult Continuations(string client)
    {
      string jobId = BackgroundJob.Enqueue(() =>
          Console.WriteLine($"Check balance logic for {client}"));

      BackgroundJob.ContinueJobWith(jobId, () =>
          Console.WriteLine($"{client}, your balance has been changed."));

      return Ok();
    }
  }
}
