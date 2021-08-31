using System.Linq;
using System.Security.Claims;

namespace Core.Extensions
{
  public static class ClaimsPrincipalExtension
  {

    public static string RetrieveEmailFromPrincipal(this ClaimsPrincipal user)
    {
      return user?.Claims?.FirstOrDefault(x => x.Type == ClaimTypes.Email)?.Value;

    }

  }
}