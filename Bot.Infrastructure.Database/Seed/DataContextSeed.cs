using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Core.Models;
using Bot.Infrastructure.Database;

namespace Infrastructure.Database.SeedData
{
  public class DataContextSeed
  {

    public static async Task SeedDataAsync(AppDbContext context, ILoggerFactory loggerFactory)
    {
      try
      {

        var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        if (!context.Regions.Any())
        {
          var itemsData = File.ReadAllText(path + @"/Seed/SeedData/regions.json");
          var items = JsonSerializer.Deserialize<List<Region>>(itemsData);
          foreach (var item in items)
          {
            context.Regions.Add(item);
          }
          await context.SaveChangesAsync();
        }


        if (!context.ItemTypes.Any())
        {
          var itemsData = File.ReadAllText(path + @"/Seed/SeedData/types.json");
          var items = JsonSerializer.Deserialize<List<ItemType>>(itemsData);
          foreach (var item in items)
          {
            context.ItemTypes.Add(item);
          }
          await context.SaveChangesAsync();
        }


        if (!context.BankOffices.Any())
        {
          var itemsData = File.ReadAllText(path + @"/Seed/SeedData/bankoffices.json");
          var items = JsonSerializer.Deserialize<List<Office>>(itemsData);
          foreach (var item in items)
          {
            context.BankOffices.Add(item);
          }
          await context.SaveChangesAsync();
        }

        if (!context.Members.Any())
        {
          var itemsData = File.ReadAllText(path + @"/Seed/SeedData/members.json");
          var items = JsonSerializer.Deserialize<List<Member>>(itemsData);
          foreach (var item in items)
          {
            context.Members.Add(item);
          }
          await context.SaveChangesAsync();
        }


        if (!context.Items.Any())
        {
          var itemsData = File.ReadAllText(path + @"/Seed/SeedData/items.json");
          var items = JsonSerializer.Deserialize<List<Item>>(itemsData);


          foreach (var item in items)
          {
            context.Items.Add(item);
          }
          await context.SaveChangesAsync();
        }


      }
      catch (Exception ex)
      {
        var logger = loggerFactory.CreateLogger<DataContextSeed>();
        logger.LogError(ex.Message);
      }
    }


  }
}