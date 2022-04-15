using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Core.Identity;
using Core.Models.Identity;
using Bot.Identity;
using Bot.Identity.Database;

namespace Identity.Database.SeedData
{
  public class IdentityContextSeed
  {
    public static async Task SeedUsersAsync(UserManager<AppUser> userManager, RoleManager<Role> roleManager, ILoggerFactory loggerFactory, IdentityContext context)
    {
      var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

      try
      {

        // if (!context.UserPositions.Any())
        // {
        //   var itemsData = File.ReadAllText(path + @"/Data/SeedData/Source/userpositions.json");
        //   var items = JsonSerializer.Deserialize<List<UserPosition>>(itemsData);
        //   foreach (var item in items)
        //   {
        //     context.UserPositions.Add(item);
        //   }
        //   await context.SaveChangesAsync();
        // }


        if (!userManager.Users.Any())
        {

          var itemsData = File.ReadAllText(path + @"/Seed/SeedData/users.json");
          var users = JsonSerializer.Deserialize<List<AppUser>>(itemsData);
          foreach (var user in users)
          {
            var result = userManager.CreateAsync(user, "Pa$$w0rd").Result;
          }

        }

        if (!roleManager.Roles.Any())
        {
          var roles = new List<Role>()
          {
            new Role {Name = "User", RoleName = "Пользователь"},
            new Role {Name = "Admin", RoleName = "Администратор"},
            new Role {Name = "Curator", RoleName = "Куратор"},

          };

          foreach (var role in roles)
          {
            await roleManager.CreateAsync(role);
          }

          var admin = userManager.FindByNameAsync("admin@admin.ru").Result;
          await userManager.AddToRolesAsync(admin, new[] { "Admin", "User" });

          var curator = userManager.FindByNameAsync("curator@curator.ru").Result;
          await userManager.AddToRolesAsync(curator, new[] { "Curator", "User" });

        }



      }
      catch (Exception ex)
      {
        var logger = loggerFactory.CreateLogger<IdentityContextSeed>();
        logger.LogError(ex.Message);
      }
    }
  }
}