using System;
using System.Collections.Generic;

namespace Core.Models
{
  public class Item : BaseEntity
  {

    public Item()
    {
    }

    public Item(
      string messageText,
      int? authorId,
      int itemTypeId,
      ItemType itemType,
      string chatId,
      string name,
      string? jobId
    )
    {
      MessageText = messageText;
      AuthorId = authorId;
      ItemType = itemType;
      ItemTypeId = itemTypeId;
      ChatId = chatId;
      Name = name;
      JobId = jobId;
    }


    public string MessageText { get; set; }
    public int? AuthorId { get; set; }
    public string? ChatId { get; set; }
    public string? JobId { get; set; }
    public string Name { get; set; }
    public ItemType? ItemType { get; set; }
    public int? ItemTypeId { get; set; }
    public DateTime EnrolledDate { get; set; } = DateTime.Now;
    public virtual ICollection<ItemChat> ItemChats { get; set; }

  }


}

