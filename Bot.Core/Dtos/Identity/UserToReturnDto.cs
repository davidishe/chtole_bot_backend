using System.Collections.Generic;
using Core.Models.Identity;

namespace Core.Dtos
{
  public class UserToReturnDto
  {
    public int? Id { get; set; }
    public string Email { get; set; }
    public string DisplayName { get; set; }
    public string? PictureUrl { get; set; }
    public string? UserPosition { get; set; }
    public int? UserPositionId { get; set; }
    public string? BankOffice { get; set; }
    public int? BankOfficeId { get; set; }
    public string? UserDescription { get; set; }
    public string? Token { get; set; }
    public IList<string>? UserRoles { get; set; }


  }
}