using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Identity
{
  public class Address
  {
    public int Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Street { get; set; }
    public string City { get; set; }
    public string House { get; set; }
    public string ZipCode { get; set; }
    public int AppUserId { get; set; }

    [ForeignKey("AppUserId")]
    public AppUser AppUser { get; set; }

  }
}