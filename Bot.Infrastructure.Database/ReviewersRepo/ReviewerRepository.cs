using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bot.Infrastructure.Database;
using Core.Models;
using Microsoft.EntityFrameworkCore;

namespace Bot.Infrastructure
{
  public class ReviewerRepository : IReviewerRepository
  {

    private readonly AppDbContext _context;

    public ReviewerRepository(AppDbContext context)
    {
      _context = context;
    }

    public void Add<T>(T entity) where T : class
    {
      _context.Add(entity);
    }

    public void Delete<T>(T entity) where T : class
    {
      _context.Remove(entity);
    }

    public async void Update(Reviewer entity)
    {
      // _context.Reviewers.Attach(entity);
      // _context.Entry(entity).State = EntityState.Modified;
      _context.Update(entity);
      // await _context.SaveChangesAsync();
    }

    public async Task<Reviewer> GetReviewerByIdAsync(int id)
    {
      var reviewer = await _context.Reviewers.Include(p => p.Id).FirstOrDefaultAsync();
      return reviewer;
    }

    public async Task<bool> SaveAll()
    {
      return await _context.SaveChangesAsync() > 0;
    }

    public async Task<IQueryable<Reviewer>> GetReviewers()
    {
      var reviewers = _context.Reviewers.AsQueryable();
      return reviewers;
    }
  }
}