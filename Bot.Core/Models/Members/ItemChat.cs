using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Bot.Core.Dtos;
using Newtonsoft.Json;

namespace Core.Models
{
  public class ItemChat : BaseEntity
  {

    public Item? Item { get; set; }
    public int ItemId { get; set; }
    public Chat? Chat { get; set; }
    public int ChatId { get; set; }

  }
}