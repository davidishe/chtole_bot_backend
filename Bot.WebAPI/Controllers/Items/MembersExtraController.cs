using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Core.Dtos;
using Core.Identity;
using Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using Bot.Core.Validators;
using Infrastructure.Database;
using Bot.Infrastructure.Specifications;
using Bot.Identity.Extensions;

namespace WebAPI.Controllers
{


  [AllowAnonymous]
  public class MembersExtraController : BaseApiController
  {

    private readonly IGenericRepository<Member> _membersRepo;
    private readonly IMapper _mapper;
    private readonly UserManager<HavenAppUser> _userManager;


    public MembersExtraController(
      IGenericRepository<Member> membersRepo,
      IMapper mapper,
      UserManager<HavenAppUser> userManager
    )
    {
      _membersRepo = membersRepo;
      _mapper = mapper;
      _userManager = userManager;
    }

    #region 1. Get products functionality


    [AllowAnonymous]
    [HttpGet]
    [Route("all")]
    public async Task<ActionResult<IReadOnlyList<MemberDto>>> GetAllMembers()
    {

      var spec = new MemberSpecification();
      var items = await _membersRepo.ListAsync(spec);

      var data = _mapper.Map<IReadOnlyList<Member>, IReadOnlyList<MemberDto>>(items);
      await SetTimeOut();
      return Ok(data);
    }


    [AllowAnonymous]
    [HttpGet("{id}")]
    [Route("getbyid")]
    public async Task<ActionResult<MemberDto>> GetByIdAsync([FromQuery] int id)
    {
      var spec = new MemberSpecification(id);
      var item = await _membersRepo.GetEntityWithSpec(spec);
      await SetTimeOut();

      var resultDto = _mapper.Map<Member, MemberDto>(item);
      return resultDto;

    }


    #endregion



    #region 3. Products CRUD functionality
    /*
    create, delete, update products
    */


    [AllowAnonymous]
    [HttpPost]
    [Route("create")]
    public async Task<ActionResult<Member>> Create(MemberDto itemDto)
    {

      var user = await _userManager.FindByClaimsCurrentUser(HttpContext.User);
      await SetTimeOut();

      MemberValidator validator = new MemberValidator();
      var validationResults = validator.Validate(itemDto);

      //TODO: реализовать возврат ошибок в виде error response object
      if (validationResults.IsValid == false)
      {
        foreach (var errorResult in validationResults.Errors)
        {
          return Ok($"{errorResult.PropertyName}: {errorResult.ErrorMessage}");
        }
      }


      var item = new Member
      (
        name: itemDto.Name,
        isEnabled: true,
        birthdayDate: new DateTime()
      // TODO: get id from user object
      // authorId: 2,
      );

      var itemToReturn = await _membersRepo.AddEntityAsync(item);
      return Ok(itemToReturn);

    }





    [AllowAnonymous]
    [HttpPut]
    [Route("update")]
    public async Task<ActionResult<Member>> UpdateProduct(MemberDto itemForUpdate)
    {

      var currentItem = _membersRepo.GetByIdAsync((int)itemForUpdate.Id).Result;

      if (currentItem == null)
        return BadRequest("Не найден объект для обновления");

      currentItem.IsEnabled = itemForUpdate.IsEnabled;
      currentItem.Name = itemForUpdate.Name;
      await SetTimeOut();

      var updatedItem = _membersRepo.Update(currentItem);
      if (updatedItem != null)
        return Ok(_mapper.Map<Member, MemberDto>(updatedItem));
      else
        return NotFound();

    }




    [AllowAnonymous]
    [HttpDelete]
    [Route("delete")]
    public async Task<ActionResult> DeleteProduct([FromQuery] int id)
    {

      await SetTimeOut();
      var item = await _membersRepo.GetByIdAsync(id);
      if (item != null)
      {
        _membersRepo.Delete(item);
        return Ok(202);
      }

      return BadRequest();

    }

    #endregion


    #region 5. Private methods for service functionaluty

    private async Task<bool> SetTimeOut()
    {
      await Task.Delay(100);
      return true;
    }

    #endregion

  }
}