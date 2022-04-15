using System;

namespace Core.Models
{
  public class Reviewer : BaseEntity
  {

    public Reviewer()
    {
    }

    public string Name { get; set; }
    public string UserName { get; set; }
    public DateTime LastReviewDate { get; set; } = DateTime.Now.AddDays(GetRnd());
    public bool Status { get; set; }



    private static int GetRnd()
    {
      Random rnd = new Random();
      var value = rnd.Next(1000);
      return value;
    }

  }
}