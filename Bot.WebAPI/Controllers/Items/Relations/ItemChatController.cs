using System.Threading.Tasks;
using Bot.Core.Dtos;
using Core.Models;
using Infrastructure.Database;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Controllers;

namespace Bot.WebAPI.Controllers.Items
{

  [AllowAnonymous]
  public class ItemChatController_ : BaseApiController
  {


    private readonly IGenericRepository<ItemChat> _itemChatRepo;
    private readonly IGenericRepository<Chat> _chatRepo;
    private readonly IGenericRepository<Item> _itemRepo;


    public ItemChatController_(
        IGenericRepository<ItemChat> itemChatRepo,
        IGenericRepository<Chat> chatRepo,
        IGenericRepository<Item> itemRepo
    )
    {
      _itemChatRepo = itemChatRepo;
      _chatRepo = chatRepo;
      _itemRepo = itemRepo;

    }


    [AllowAnonymous]
    [HttpPost]
    [Route("create")]
    public async Task<ActionResult<ItemChat>> Create(ItemChatDto itemChatDto)
    {


      var item = new ItemChat
      {
        ChatId = itemChatDto.ChatId,
        Chat = await _chatRepo.GetByIdAsync(itemChatDto.ChatId),
        ItemId = itemChatDto.ItemId,
        Item = await _itemRepo.GetByIdAsync(itemChatDto.ItemId)
      };

      var itemToReturn = await _itemChatRepo.AddEntityAsync(item);
      return Ok(itemToReturn);

    }

  }
}