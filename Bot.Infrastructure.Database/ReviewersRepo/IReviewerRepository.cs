using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Helpers;
using Core.Models;

namespace Bot.Infrastructure
{
  public interface IReviewerRepository
  {
    void Add<T>(T entity) where T : class;
    Task<bool> SaveAll();
    void Delete<T>(T entity) where T : class;
    Task<Reviewer> GetReviewerByIdAsync(int id);
    Task<IQueryable<Reviewer>> GetReviewers();
    void Update(Reviewer entity);



  }
}