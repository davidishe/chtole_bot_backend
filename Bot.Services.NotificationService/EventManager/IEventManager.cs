using System;
using System.Threading.Tasks;

namespace EventService
{
  public interface IEventManager
  {
    Task ExecuteRegularEvent(string jobId);
    Task<bool> SetHappyBirthdayEvent(string jobId);



  }

}