using System.Threading.Tasks;
using Core.Identity;

namespace Bot.Identity
{
  public interface ITokenService
  {
    Task<string> CreateToken(AppUser user);
  }
}