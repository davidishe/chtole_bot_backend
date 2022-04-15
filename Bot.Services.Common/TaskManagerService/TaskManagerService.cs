using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;
using Bot.Infrastructure;
using System.Linq;
using System;
using Core.Models;
using Bot.Infrastructure.Database;

namespace Infrastructure.Services.TaskManagerService
{
  public class TaskManagerService : ITaskManagerService
  {

    private readonly string _telegramToken;
    ILogger<TaskManagerService> _logger;

    private readonly IDbRepository<Reviewer> _reviwersRepo;


    public TaskManagerService(
      IConfiguration config,
      IDbRepository<Reviewer> reviwersReppo,
      ILogger<TaskManagerService> logger
    )
    {
      _telegramToken = config.GetSection("BotSettings:TelegramToken").Value;
      _logger = logger;
      _reviwersRepo = reviwersReppo;
    }



    public async Task CreateTask(long chatId, Message message)
    {



    }
  }
}