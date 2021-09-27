// using System.Collections.Generic;
// using System.Threading.Tasks;
// using Bot.Core.Dtos;
// using Bot.Infrastructure.Specifications;
// using Core.Models;
// using Infrastructure.Database;
// using Microsoft.AspNetCore.Authorization;
// using Microsoft.AspNetCore.Mvc;
// using Microsoft.Extensions.Logging;

// namespace WebAPI.Controllers
// {

//   [ApiController]
//   [AllowAnonymous]
//   [Route("api/[controller]")]
//   public class BaseRelationsController<TEntity, TObject, TEntityObject, TDto> : ControllerBase
//     where TEntity : BaseEntity
//     where TObject : BaseEntity
//     where TEntityObject : BaseEntity
//     where TDto : BaseDto
//   {

//     private readonly IGenericRepository<TEntity> _entityRepo;
//     private readonly IGenericRepository<TObject> _objectRepo;
//     private readonly IGenericRepository<TEntityObject> _entityObjectRepo;
//     private readonly ILogger<BaseRelationsController<TEntity, TObject, TEntityObject, TDto>> _logger;

//     public BaseRelationsController(
//       IGenericRepository<TEntity> entityRepo,
//       IGenericRepository<TObject> objectRepo,
//       IGenericRepository<TEntityObject> entityObjectRepo,
//       ILogger<BaseRelationsController<TEntity, TObject, TEntityObject, TDto>> logger
//     )
//     {
//       _entityRepo = entityRepo;
//       _objectRepo = objectRepo;
//       _entityObjectRepo = entityObjectRepo;
//       _logger = logger;
//     }


//     [HttpGet]
//     [Route("all")]
//     public async Task<ActionResult<IReadOnlyList<TEntity>>> GetAllAsync()
//     {
//       await SetTimeOut();
//       var spec = new BaseSpecification<TEntity>();
//       var entitys = await _entityRepo.ListAsync(spec);
//       return Ok(entitys);
//     }


//     [HttpGet]
//     [Route("getbyid")]
//     public async Task<ActionResult<IReadOnlyList<TEntity>>> GetById([FromQuery] int id)
//     {
//       await SetTimeOut();
//       var entity = await _entityRepo.GetByIdAsync(id);
//       return Ok(entity);
//     }


//     [HttpPost]
//     [Route("create")]
//     public async Task<ActionResult<TEntity>> Create(TDto dto)
//     {

//       if (dto == null)
//         return BadRequest("Вы отправили пустой объект");




//       var entityToReturn = await _entityObjectRepo.AddEntityAsync(entity as TEntityObject);
//       return Ok(entityToReturn);

//     }








//     [HttpPut]
//     [Route("update")]
//     public async Task<ActionResult<IReadOnlyList<TEntity>>> Update(TEntity entity)
//     {
//       await SetTimeOut();
//       if (entity == null)
//         return BadRequest("Вы отправили нам пустой объект");

//       if (!(entity.Id >= 1))
//         return BadRequest("У объекта должен быть id");

//       _entityRepo.Update(entity);
//       _logger.LogInformation("Значение было обновлено");
//       return Ok(entity);
//     }


//     [HttpDelete]
//     [Route("delete")]
//     public async Task<ActionResult<IReadOnlyList<TEntity>>> Delete([FromQuery] int id)
//     {
//       await SetTimeOut();
//       var entity = await _entityRepo.GetByIdAsync(id);
//       _entityRepo.Delete(entity);
//       return Ok(entity);
//     }


//     private async Task<bool> SetTimeOut()
//     {
//       await Task.Delay(10);
//       return true;
//     }


//   }
// }