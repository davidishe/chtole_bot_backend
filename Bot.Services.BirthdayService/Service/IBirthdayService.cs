using System.Threading.Tasks;

namespace Bot.Services.BirthdayService
{
  public interface IBirthdayService
  {
    Task SetBirthdayJobAsync();
  }
}