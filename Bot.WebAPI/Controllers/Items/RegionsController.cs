using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Core.Models;
using Infrastructure.Database;
using Microsoft.Extensions.Logging;

namespace WebAPI.Controllers
{


  [AllowAnonymous]
  public class RegionsController : BaseController<Region>
  {


    public RegionsController(
      IGenericRepository<Region> context,
      ILogger<RegionsController> logger
    ) : base(context, logger)
    {
    }


    [AllowAnonymous]
    [HttpGet]
    [Route("alltest")]
    public async Task<ActionResult<List<Region>>> GetRegionsAsyncTest()
    {
      Region[] items = new Region[] { new Region { Name = "Вологда" }, new Region { Name = "Калуга" }, new Region { Name = "Пенза" } };
      return Ok(items);
    }


  }
}