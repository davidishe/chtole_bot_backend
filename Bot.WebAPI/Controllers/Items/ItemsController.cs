using System.Collections.Generic;
using System.IO;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using AutoMapper;
using Core.Dtos;
using Infrastructure.Helpers;
using Core.Identity;
using Core.Models;
using Infrastructure.Data.Repos.GenericRepository;
using Infrastructure.Data.Spec;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using Core.Extensions;
using NotificationService.JobManagment;
using NotificationService.Notification;

namespace WebAPI.Controllers
{


  [AllowAnonymous]
  public class ItemsController : BaseApiController
  {

    private readonly IGenericRepository<Item> _itemsRepo;
    private readonly IGenericRepository<ItemType> _itemTypeRepo;
    private readonly IGenericRepository<ItemSubType> _itemSubTypeRepo;
    private readonly IGenericRepository<Office> _officeRepo;
    private readonly IMapper _mapper;
    private readonly UserManager<HavenAppUser> _userManager;
    private readonly IJobManager _jobManager;
    private readonly INotificationManager _notificationManager;



    public ItemsController(
      IGenericRepository<Item> productsRepo,
      IGenericRepository<ItemType> productTypeRepo,
      IGenericRepository<ItemSubType> itemSubTypeRepo,
      IGenericRepository<Office> officeRepo,
      IMapper mapper,
      UserManager<HavenAppUser> userManager,
      IJobManager jobManager,
      INotificationManager notificationManager
    )
    {
      _itemsRepo = productsRepo;
      _itemTypeRepo = productTypeRepo;
      _itemSubTypeRepo = itemSubTypeRepo;
      _mapper = mapper;
      _officeRepo = officeRepo;
      _userManager = userManager;
      _jobManager = jobManager;
      _notificationManager = notificationManager;
    }


    #region 1. Get products functionality

    [Authorize(Policy = "RequireModerator")]
    [HttpGet]
    [Route("all/admin")]
    public async Task<ActionResult> GetAllAdmin([FromQuery] UserParams userParams)
    {

      await SetTimeOut();
      userParams.IsAdmin = true;

      var specForCount = new DocForCountSpecification(userParams);
      var totalItems = await _itemsRepo.CountAsync(specForCount);

      var spec = new ItemSpecification(userParams);

      var products = await _itemsRepo.ListAsync(spec);
      var data = _mapper.Map<IReadOnlyList<Item>, IReadOnlyList<ItemDto>>(products);

      return Ok(new Pagination<ItemDto>(userParams.PageIndex, userParams.PageSize, totalItems, data));
    }

    [AllowAnonymous]
    [HttpGet]
    [Route("all")]
    public async Task<ActionResult<Pagination<ItemDto>>> GetAllClient([FromQuery] UserParams userParams)
    {

      userParams.IsAdmin = false;

      var specForCount = new DocForCountSpecification(userParams);
      var totalItems = await _itemsRepo.CountAsync(specForCount);
      var spec = new ItemSpecification(userParams);
      var items = await _itemsRepo.ListAsync(spec);

      var data = _mapper.Map<IReadOnlyList<Item>, IReadOnlyList<ItemDto>>(items);
      await SetTimeOut();
      return Ok(new Pagination<ItemDto>(userParams.PageIndex, userParams.PageSize, totalItems, data));
    }


    [AllowAnonymous]
    [HttpGet("{id}")]
    [Route("getbyid")]
    public async Task<ActionResult<ItemDto>> GetProductByIdAsync([FromQuery] int id)
    {
      var spec = new ItemSpecification(id);
      var item = await _itemsRepo.GetEntityWithSpec(spec);
      var cronExpression = _jobManager.GetCronExpressionByJobId(item.JobId);
      await SetTimeOut();

      var resultDto = _mapper.Map<Item, ItemDto>(item);
      resultDto.CronExpression = cronExpression;
      return resultDto;

    }

    [AllowAnonymous]
    [HttpGet("{guId}")]
    [Route("getproductid")]
    public async Task<ActionResult<ItemDto>> GetProductIdByGuIdAsync([FromQuery] int id)
    {
      var product = await _itemsRepo.GetByIdAsync(id);
      await SetTimeOut();

      var type = _itemTypeRepo.GetByIdAsync((int)product.ItemTypeId).Result;
      product.ItemType = type;

      return _mapper.Map<Item, ItemDto>(product);
    }

    #endregion

    #region 2. Get regions & types functionality
    [AllowAnonymous]
    [HttpGet]
    [Route("types")]
    public async Task<ActionResult<IReadOnlyList<ItemType>>> GetProductTypesByIdAsync()
    {
      var spec = new BaseSpecification<ItemType>();
      var product = await _itemTypeRepo.ListAsync(spec);
      await SetTimeOut();
      return Ok(product);
    }

    [AllowAnonymous]
    [HttpGet]
    [Route("subtypes")]
    public async Task<ActionResult<IReadOnlyList<ItemSubType>>> GetByIdAsync()
    {
      var spec = new BaseSpecification<ItemSubType>();
      var item = await _itemSubTypeRepo.ListAsync(spec);
      await SetTimeOut();
      return Ok(item);
    }
    #endregion

    #region 3. Products CRUD functionality
    /*
    create, delete, update products
    */


    [Authorize]
    [HttpPost]
    [Route("create")]
    public async Task<ActionResult<Item>> Create(ItemDto itemDto)
    {

      var user = await _userManager.FindByClaimsCurrentUser(HttpContext.User);
      await SetTimeOut();

      var jobId = _jobManager.AddRecurringJob(itemDto.CronExpression);

      var item = new Item
      (
        messageText: itemDto.MessageText,
        name: itemDto.Name,
        jobId: jobId,
        // TODO: get id from user object
        authorId: 2,
        chatId: itemDto.ChatId,
        itemTypeId: (int)itemDto.ItemTypeId,
        itemType: _itemTypeRepo.GetByIdAsync((int)itemDto.ItemTypeId).Result
      );

      var itemToReturn = await _itemsRepo.AddEntityAsync(item);
      return Ok(itemToReturn);

    }


    [AllowAnonymous]
    [HttpPut]
    [Route("update")]
    public async Task<ActionResult<Item>> UpdateProduct(ItemDto itemForUpdate)
    {

      var currentItem = _itemsRepo.GetByIdAsync((int)itemForUpdate.Id).Result;

      if (currentItem == null)
        return BadRequest("Не найден объект для обновления");

      currentItem.MessageText = itemForUpdate.MessageText;
      currentItem.Name = itemForUpdate.Name;
      currentItem.JobId = itemForUpdate.JobId;
      currentItem.ChatId = itemForUpdate.ChatId;
      currentItem.ItemTypeId = itemForUpdate.ItemTypeId;
      currentItem.ItemType = _itemTypeRepo.GetByIdAsync((int)itemForUpdate.ItemTypeId).Result;
      await SetTimeOut();

      var updatedItem = _itemsRepo.Update(currentItem);

      var updatedJobResult = _jobManager.UpdateRecurringJob(itemForUpdate.JobId, itemForUpdate.CronExpression);
      if (updatedJobResult == true)
        return Ok(_mapper.Map<Item, ItemDto>(updatedItem));
      else
        return BadRequest();

    }




    [AllowAnonymous]
    [HttpDelete]
    [Route("delete")]
    public async Task<ActionResult> DeleteProduct([FromQuery] int id)
    {

      await SetTimeOut();
      var item = await _itemsRepo.GetByIdAsync(id);
      var result = _jobManager.DeleteRecurringJob(item.JobId);
      if (result)
      {
        _itemsRepo.Delete(item);
        return Ok(202);
      }

      return BadRequest(result);

    }

    #endregion


    #region 5. Private methods for service functionaluty
    private Task<string> SaveFileToServer(IFormFile file)
    {
      var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
      var fullPath = Path.Combine(Directory.GetCurrentDirectory(), "assets", "images", "products", fileName);

      using (var stream = new FileStream(fullPath, FileMode.Create))
      {
        file.CopyTo(stream);
      }

      var fileDirecoryToReturn = Path.Combine("assets", "images", "products", fileName);

      return Task.FromResult(fileDirecoryToReturn);

    }

    private async Task<bool> SetTimeOut()
    {
      await Task.Delay(100);
      return true;
    }

    #endregion

  }
}