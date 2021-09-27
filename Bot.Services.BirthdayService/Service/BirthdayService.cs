using System;
using System.Linq;
using System.Threading.Tasks;
using Bot.Infrastructure.Specifications;
using Core.Models;
using Infrastructure.Database;

namespace Bot.Services.BirthdayService
{
  public class BirthdayService : IBirthdayService
  {

    private readonly IGenericRepository<Member> _membersRepo;

    public BirthdayService(IGenericRepository<Member> membersRepo)
    {
      _membersRepo = membersRepo;
    }

    public async Task SetBirthdayJobAsync()
    {
      var members = GetBirthdayMembers();
      throw new System.NotImplementedException();
    }


    private async Task<Member[]> GetBirthdayMembers()
    {

      var spec = new BaseSpecification<Member>();
      var members = await _membersRepo.ListAsync(spec);
      var memberWithBirthday = members.Where(x => x.BirthdayDate.Date.Month == DateTime.Now.Date.Month && x.BirthdayDate.Date.Day == DateTime.Now.Date.Day);
      var membersArray = memberWithBirthday.ToArray();
      return membersArray;
    }

  }
}