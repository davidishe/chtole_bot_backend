using System.ComponentModel.DataAnnotations;

namespace Core.Dtos
{
  public class UserToLoginDto
  {
    public string Email { get; set; }
    public string Password { get; set; }
  }
}