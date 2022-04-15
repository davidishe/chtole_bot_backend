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
      if ((dayToday != DayOfWeek.Saturday) && (dayToday != DayOfWeek.Sunday) && !item.Status)
        _telegramService.SendMessage(item.ChatId, messageToSend);

      return Task.CompletedTask;
    }


    public async Task<bool> SetHappyBirthdayEvent(string jobId)
    {

      var spec = new ItemSpecification();
      var items = _itemsRepo.ListAsync(spec).Result;
      var item = items.Where(x => x.JobId == jobId).FirstOrDefault();

      var members = await GetBirthdayMembers();

      if (members == null)
        _logger.LogInformation($"oops, there is no chat members with birthday!");

      foreach (var member in members)
      {
        var message = item.MessageText;
        string outputMessage = message.Replace("{человек}", member.Name);
        _logger.LogInformation(outputMessage);



        //TODO: взять все чаты где данный member участвует и отправить туда сообщения

      }
      return true;
    }


    private async Task<Member[]> GetBirthdayMembers()
    {

      var spec = new BaseSpecification<Member>();
      var members = await _membersRepo.ListAsync(spec);
      var memberWithBirthday = members.Where(x => x.BirthdayDate.Date.Month == DateTime.Now.Date.Month && x.BirthdayDate.Date.Day == DateTime.Now.Date.Day);
      var membersArray = memberWithBirthday.ToArray();
      return membersArray;
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