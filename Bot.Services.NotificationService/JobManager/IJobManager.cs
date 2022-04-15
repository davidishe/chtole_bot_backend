
using System.Threading.Tasks;

namespace NotificationService.JobManagment
{

  public interface IJobManager
  {
    string AddRecurringJob(string cronExpression);
    bool UpdateRecurringJob(string jobId, string cronExpression);
    string GetCronExpressionByJobId(string jobId);
    bool DeleteRecurringJob(string jobId);

    public string AddHappyBirthdayJob(string cronExpression);



  }

}
