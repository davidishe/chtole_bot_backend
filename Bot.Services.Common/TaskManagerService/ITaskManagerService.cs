using System.Threading.Tasks;
using Core.Dtos;
using Core.Models;
using Telegram.Bot.Args;
using Telegram.Bot.Types;

namespace Infrastructure.Services.TaskManagerService
{
  public interface ITaskManagerService
  {
    Task CreateTask(long chatId, Message e);

  }
}