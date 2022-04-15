using System.Collections.Generic;
using System.Threading.Tasks;
using Bot.Infrastructure.Specifications;
using Core.Models;

namespace Infrastructure.Database

{
  public interface IGenericRepository<T> where T : BaseEntity
  {
    Task<T> GetByIdAsync(int id);
    Task<T> GetEntityWithSpec(ISpecification<T> spec);
    Task<IReadOnlyList<T>> ListAsync(ISpecification<T> spec);

    Task<IReadOnlyList<T>> GetAllAsync();

    Task<int> CountAsync(ISpecification<T> spec);
    Task<T> AddEntityAsync(T entity);
    T Update(T entity);
    bool Delete(T entity);


  }
}