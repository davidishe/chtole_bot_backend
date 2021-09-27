using Core.Models;
using Infrastructure.Database;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using WebAPI.Controllers;

namespace Bot.WebAPI.Controllers
{

  [AllowAnonymous]
  public class ChatsController : BaseController<Chat>
  {

    public ChatsController(
      IGenericRepository<Chat> context,
      ILogger<ChatsController> logger
    ) : base(context, logger)
    {
    }
  }
}