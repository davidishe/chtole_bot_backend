using System.Linq;
using System.Threading.Tasks;
using Core.Models;
using Microsoft.EntityFrameworkCore;

namespace Bot.Infrastructure.Database
{
  public class DbRepository<TEntity> : IDbRepository<TEntity> where TEntity : BaseEntity
  {
    private readonly AppDbContext _context;

    public DbRepository(AppDbContext context)
    {
      _context = context;
    }

    /// <inheritdoc />
    public IQueryable<TEntity> GetAll()
    {
      var result = _context.Set<TEntity>().AsQueryable();
      return result;
    }

    /// <inheritdoc />
    public async Task<TEntity> AddAsync(TEntity entity)
    {
      await _context.Set<TEntity>().AddAsync(entity);
      await _context.SaveChangesAsync();
      return entity;
    }

    /// <inheritdoc />
    public async Task UpdateAsync(TEntity entity)
    {
      // _context.Set<TEntity>().Update(entity);
      // _context.Entry(entity).State = EntityState.Modified;
      // await _context.SaveChangesAsync();

      // await Task.Run(() => _context.Set<TEntity>().Update(entity));
      // await _context.SaveChangesAsync();

      await Task.Run(() => _context.Set<TEntity>().Update(entity));
      await _context.SaveChangesAsync();

    }


    /// <inheritdoc />
    public async Task SaveAsync()
    {
      await _context.SaveChangesAsync();
    }


    /// <inheritdoc />
    public async Task DeleteAsync(TEntity entity)
    {
      await Task.Run(() => _context.Set<TEntity>().Remove(entity));
      await _context.SaveChangesAsync();
    }


    /// <inheritdoc />
    public async Task<TEntity> GetByIdAsync(int id)
    {
      var entity = await Task.Run(() => _context.Set<TEntity>().Where(x => x.Id == id).FirstOrDefault());
      await _context.SaveChangesAsync();
      return entity;
    }


  }
}