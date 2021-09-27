using System;
using Core.Models;
using Core.Helpers;

namespace Bot.Infrastructure.Specifications
{
  public class MemberSpecification : BaseSpecification<Member>
  {
    public MemberSpecification(UserParams userParams)
    : base(x =>
          (string.IsNullOrEmpty(userParams.Search) || x.Name.ToLower().Contains(userParams.Search.ToLower())))
    {
      AddInclude(x => x.MemberChats);

      ApplyPaging(userParams.PageSize * userParams.PageIndex, userParams.PageSize);

      if (!string.IsNullOrEmpty(userParams.sort))
      {
        switch (userParams.sort)
        {
          case "name":
            AddOrderByAscending(s => s.Name);
            break;
          default:
            AddOrderByAscending(x => x.IsEnabled);
            break;
        }
      }
    }


    public MemberSpecification()
    : base()
    {
      AddInclude(x => x.MemberChats);
    }



    public MemberSpecification(int id) : base(x => x.Id == id)
    {
      AddInclude(x => x.MemberChats);
    }
  }


}