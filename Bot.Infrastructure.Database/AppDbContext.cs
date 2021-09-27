using Bot.Core.Models.Members;
using Core.Models;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace Bot.Infrastructure.Database
{
  public class AppDbContext : DbContext
  {
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
      Database.EnsureCreated();
    }

    public DbSet<Office> BankOffices { get; set; }
    public DbSet<ItemType> ItemTypes { get; set; }
    public DbSet<ItemSubType> ItemSubTypes { get; set; }
    public DbSet<Item> Items { get; set; }
    public DbSet<Region> Regions { get; set; }
    public DbSet<Member> Members { get; set; }
    // public DbSet<ItemChat> ItemChats { get; set; }
    // public DbSet<MemberChat> MemberChats { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

      modelBuilder.Entity<ItemChat>()
          .HasKey(cs => new { cs.ChatId, cs.ItemId });

      modelBuilder.Entity<MemberChat>()
        .HasKey(cs => new { cs.ChatId, cs.MemberId });

      base.OnModelCreating(modelBuilder);
      modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }

  }

}
