using Microsoft.AspNetCore.Identity;
using Core.Identity;

namespace Core.Models.Identity
{
  public class UserRole : IdentityUserRole<int>
  {
    public virtual AppUser User { get; set; }
    public virtual Role Role { get; set; }

  }
}