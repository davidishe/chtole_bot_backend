using System;
using System.Linq;
using System.Threading.Tasks;
using Bot.Infrastructure.Specifications;
using Core.Models;
using Infrastructure.Database;
using Infrastructure.Services.TelegramService;
using Microsoft.Extensions.Logging;

namespace EventService.Event
{

  public class EventManager : IEventManager
  {
    private readonly ITelegramService _telegramService;
    private readonly IGenericRepository<Item> _itemsRepo;
    private readonly IGenericRepository<Member> _membersRepo;
    private readonly ILogger<EventManager> _logger;


    public EventManager(
      ITelegramService telegrammService,
      ILogger<EventManager> logger,
      IGenericRepository<Item> itemRepo,
      IGenericRepository<Member> membersRepo

    )
    {
      _telegramService = telegrammService;
      _itemsRepo = itemRepo;
      _logger = logger;
      _membersRepo = membersRepo;
    }

    public Task ExecuteRegularEvent(string jobId)
    {
      var spec = new ItemSpecification();
      var items = _itemsRepo.ListAsync(spec).Result;
      var item = items.Where(x => x.JobId == jobId).FirstOrDefault();
      var messageToSend = GetRegularMessageWithSpeakerAsync(item.MessageText).Result;
      _logger.LogInformation($"{DateTime.Now} было отправлено сообщение {messageToSend} в чат {item.ChatId}");

      DayOfWeek dayToday = DateTime.Now.DayOfWeek;
      if ((dayToday != DayOfWeek.Saturday) && (dayToday != DayOfWeek.Sunday))
        _telegramService.SendMessage(item.ChatId, messageToSend);

      return Task.CompletedTask;
    }


    public Task SetHappyBirthdayEvent(string jobId)
    {
      var spec = new ItemSpecification();
      var items = _itemsRepo.ListAsync(spec).Result;
      var item = items.Where(x => x.JobId == jobId).FirstOrDefault();
      // var membersWithBirthday = GetMessageForBirthdayMembers().Result;

      // foreach (var member in membersWithBirthday)
      // {
      //   var messageToSend = GetRegularMessageWithSpeakerAsync(item.MessageText).Result;
      //   _logger.LogInformation($"{DateTime.Now} было отправлено сообщение {messageToSend} в чат {item.ChatId}");
      //   _telegramService.SendMessage(item.ChatId, messageToSend);

      // }


      return Task.CompletedTask;
    }





    private async Task<string> GetRegularMessageWithSpeakerAsync(string message)
    {

      var spec = new MemberSpecification();
      var members = await _membersRepo.ListAsync(spec);
      var membersArray = members.ToArray();
      var rnd = new Random();
      var rndIndex = rnd.Next(membersArray.Length);

      string output = message.Replace("{человек}", membersArray[rndIndex].Name);
      Console.WriteLine(output);
      return output;
    }







  }
}