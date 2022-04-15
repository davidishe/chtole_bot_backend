using System;
using System.Linq;
using System.Threading.Tasks;
using EventService;
using Hangfire;
using Hangfire.Storage;

namespace NotificationService.JobManagment
{

  public class JobManager : IJobManager
  {


    private readonly IEventManager _eventManager;

    public JobManager(
      IEventManager eventManager
    )
    {
      _eventManager = eventManager;
    }

    [Obsolete]
    public string AddRecurringJob(string cronExpression)
    {
      var timeZone = TimeZone.CurrentTimeZone;
      var jobId = Guid.NewGuid().ToString();
      RecurringJob.AddOrUpdate(jobId, () => _eventManager.ExecuteRegularEvent(jobId), cronExpression, TimeZoneInfo.Local);
      return jobId;
    }


    [Obsolete]
    public string AddHappyBirthdayJob(string cronExpression)
    {
      var timeZone = TimeZone.CurrentTimeZone;
      var jobId = Guid.NewGuid().ToString();
      RecurringJob.AddOrUpdate(jobId, () => _eventManager.SetHappyBirthdayEvent(jobId), cronExpression, TimeZoneInfo.Local);
      return jobId;
    }


    public bool UpdateRecurringJob(string jobId, string cronExpression)
    {
      RecurringJob.AddOrUpdate(jobId, () => _eventManager.ExecuteRegularEvent(jobId), cronExpression, TimeZoneInfo.Local);
      return true;
    }

    public bool DeleteRecurringJob(string jobId)
    {
      RecurringJob.RemoveIfExists(jobId);
      return true;
    }

    public string GetCronExpressionByJobId(string jobId)
    {
      var recurringJobs = JobStorage.Current.GetConnection().GetRecurringJobs();
      var job = recurringJobs.Where(x => x.Id == jobId).FirstOrDefault();
      if (job == null)
        return "Значение не найдено";

      var cronExpression = job.Cron;
      return cronExpression;
    }


  }
}