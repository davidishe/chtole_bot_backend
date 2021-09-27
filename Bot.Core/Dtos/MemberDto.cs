using System;
using System.Collections.Generic;
using Core.Identity;
using Core.Models;

namespace Core.Dtos
{
  public class MemberDto : BaseEntity
  {
    public string Name { get; set; }
    public bool IsEnabled { get; set; }
    public DateTime BirthdayDate { get; set; }
  };
}

