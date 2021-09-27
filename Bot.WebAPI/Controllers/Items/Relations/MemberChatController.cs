using System.Threading.Tasks;
using Bot.Core.Dtos;
using Bot.Core.Models.Members;
using Core.Models;
using Infrastructure.Database;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Controllers;

namespace Bot.WebAPI.Controllers.Items
{

  [AllowAnonymous]
  public class MemberChatController : BaseApiController
  {

    private readonly IGenericRepository<MemberChat> _memberChatRepo;
    private readonly IGenericRepository<Chat> _chatRepo;
    private readonly IGenericRepository<Member> _memberRepo;


    public MemberChatController(
        IGenericRepository<MemberChat> memberChatRepo,
        IGenericRepository<Chat> chatRepo,
        IGenericRepository<Member> memberRepo
    )
    {
      _memberChatRepo = memberChatRepo;
      _chatRepo = chatRepo;
      _memberRepo = memberRepo;

    }


    [AllowAnonymous]
    [HttpPost]
    [Route("create")]
    public async Task<ActionResult<MemberChat>> Create(MemberChatDto itemDto)
    {

      var item = new MemberChat
      {
        ChatId = itemDto.ChatId,
        Chat = await _chatRepo.GetByIdAsync(itemDto.ChatId),
        MemberId = itemDto.ChatId,
        Member = await _memberRepo.GetByIdAsync(itemDto.Memberid)
      };

      var itemToReturn = await _memberChatRepo.AddEntityAsync(item);
      return Ok(itemToReturn);

    }





  }
}