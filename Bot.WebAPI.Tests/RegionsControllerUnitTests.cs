using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Core.Dtos;
using Core.Identity;
using Core.Models;
using Infrastructure.Database;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WebAPI.Controllers;
using Xunit;

namespace Bot.WebAPI.Tests
{
  public class RegionsControllerUnitTests
  {

    private readonly RegionsController _controller;
    private readonly IGenericRepository<Region> _membersRepo;
    private readonly ILogger<RegionsController> _logger;



    public RegionsControllerUnitTests()
    {
      _controller = new RegionsController(
        _membersRepo,
        _logger
      );
    }

    [Fact]
    public void GetAllMembers_ReturnsOk()
    {

      var result = _controller.GetRegionsAsyncTest();
      Assert.IsAssignableFrom<Task<ActionResult<List<Region>>>>(result);

    }




  }
}
