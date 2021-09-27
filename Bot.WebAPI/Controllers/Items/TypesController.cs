using System.Collections.Generic;
using System.IO;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Core.Dtos;
using Infrastructure.Helpers;
using Core.Models;
using Infrastructure.Database;
using Bot.Infrastructure.Specifications;
using Microsoft.Extensions.Logging;

namespace WebAPI.Controllers
{


  [AllowAnonymous]
  public class TypesController : BaseController<ItemType>
  {


    public TypesController(
      IGenericRepository<ItemType> context,
      ILogger<TypesController> logger)
    : base(context, logger)
    {
    }

  }
}