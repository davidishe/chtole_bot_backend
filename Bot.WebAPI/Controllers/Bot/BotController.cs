using System;
using System.Linq;
using System.Threading.Tasks;
using Bot.Infrastructure.Database;
using Core.Models;
using Infrastructure.Services.TaskManagerService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace WebAPI.Controllers
{


  [ApiController]
  [Route("api")]
  public class BotController : ControllerBase
  {
    private readonly string _telegramToken;
    private readonly string _botName;

    private readonly ILogger<BotController> _logger;
    private readonly ITaskManagerService _taskManager;

    private readonly IDbRepository<Reviewer> _reviwersRepo;


    public BotController(
      IConfiguration config,
      ITaskManagerService taskManager,
      IDbRepository<Reviewer> reviwersRepo,
      ILogger<BotController> logger
    )
    {
      _telegramToken = config.GetSection("BotSettings:TelegramToken").Value;
      _botName = config.GetSection("BotSettings:BotName").Value;
      _logger = logger;
      _taskManager = taskManager;
      _reviwersRepo = reviwersRepo;
    }


    [AllowAnonymous]
    [HttpPost]
    [Route("message/update")]
    public async Task<IActionResult> Post([FromBody] Update update)
    {
      if (update == null)
        return Ok(404);

      var message = update.Message;
      if (message == null)
        return Ok(403);


      if (message.Text == null)
        return Ok("Пришло пустое сообщение");


      if (update.Message.Chat == null)
        return Ok("Нет идентификатора чата");

      var chatId = update.Message.Chat.Id;
      _logger.LogInformation(Guid.NewGuid().ToString(), null, $"Пришло сообщение на webhook {update.Message}");

      var client = new TelegramBotClient(_telegramToken);
      if ((update.Type == Telegram.Bot.Types.Enums.UpdateType.Message)
            && message.Text.Contains(_botName)
            && message.Text.ToLower().Contains("идентификатор чата"))
      {
        await client.SendTextMessageAsync(chatId, $"Идентификатор чата: {chatId}",
                                                replyToMessageId: message.MessageId);
      }

      if ((update.Type == Telegram.Bot.Types.Enums.UpdateType.Message)
      && message.Text.Contains(_botName)
      && message.Text.ToLower().Contains("список ревьюеров"))
      {
        var allReviewers = _reviwersRepo.GetAll();
        string text = "";

        foreach (var item in allReviewers)
        {
          text += item.Name + " ";
          text += "@" + item.UserName + " ";
          text += "\r\n";
        }
        await client.SendTextMessageAsync(chatId, $"{text}",
                                                replyToMessageId: message.MessageId);
      }


      if ((update.Type == Telegram.Bot.Types.Enums.UpdateType.Message)
      && message.Text.Contains(_botName)
      && message.Text.ToLower().Contains("я ревьюер"))
      {

        var allReviewers = _reviwersRepo.GetAll();
        var isReviewerExist = allReviewers.Where(z => z.UserName == message.From.Username).FirstOrDefault();
        if (isReviewerExist != null)
        {
          await client.SendTextMessageAsync(chatId, $"@{isReviewerExist.UserName} уже был добавлен в качестве ревьюера",
                                                replyToMessageId: message.MessageId);
          return Ok(200);
        }

        var reviewer = new Reviewer
        {
          Name = message.From.FirstName,
          UserName = message.From.Username,
          LastReviewDate = DateTime.Now,
          Status = true
        };

        await _reviwersRepo.AddAsync(reviewer);
        await client.SendTextMessageAsync(chatId, $"@{reviewer.UserName} добавлен в качестве ревьюера",
                                                replyToMessageId: message.MessageId);
      }


      // удаление ревьюера
      if ((update.Type == Telegram.Bot.Types.Enums.UpdateType.Message)
      && message.Text.Contains(_botName)
      && message.Text.ToLower().Contains("я ревьюер"))
      {

        var allReviewers = _reviwersRepo.GetAll();
        var reviewer = allReviewers.Where(z => z.UserName == message.From.Username).FirstOrDefault();
        await _reviwersRepo.DeleteAsync(reviewer);
        await client.SendTextMessageAsync(chatId, $"@{reviewer.UserName} удален из списка ревьюеров",
                                                replyToMessageId: message.MessageId);
      }



      if ((update.Type == Telegram.Bot.Types.Enums.UpdateType.Message)
            && message.Text.Contains(_botName)
            && message.Text.ToLower().Contains("назначь ревью"))
      {

        var reviewers = _reviwersRepo.GetAll();
        var minDate = reviewers.Max(z => z.LastReviewDate);
        var entity = reviewers.Where(z => z.LastReviewDate == minDate).FirstOrDefault();


        // TODO: фиксируем задачу в базе данных для логирования


        // назначаем на ревьюера задачу - отправляем сообщение в телегу
        await client.SendTextMessageAsync(chatId, $"Назначаю ревью с {entity.Name} @{entity.UserName}",
                                                replyToMessageId: message.MessageId);

        // обновляем ревьюеру дату последнего ревью
        entity.LastReviewDate = DateTime.Now.AddMinutes(1);
        await _reviwersRepo.UpdateAsync(entity);
      }
      return Ok(200);
    }



    [AllowAnonymous]
    [HttpGet]
    [Route("chat/members")]
    public async Task<IActionResult> GetAllMembers(string chatId)
    {
      if (chatId == null)
        return Ok(404);

      _logger.LogInformation(Guid.NewGuid().ToString(), null, $"Пришел пустой идентификатор чата");

      var client = new TelegramBotClient(_telegramToken);
      var chat = await client.GetChatAsync(chatId);

      return Ok(chat);
    }

  }
}







