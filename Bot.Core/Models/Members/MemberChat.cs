using Core.Models;

namespace Bot.Core.Models.Members
{
  public class MemberChat : BaseEntity
  {

    public int ChatId { get; set; }
    public Chat? Chat { get; set; }
    public int MemberId { get; set; }
    public Member? Member { get; set; }
  }
}