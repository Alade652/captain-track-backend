using CaptainTrackBackend.Application.Abstraction.Interface.Repository;
using CaptainTrackBackend.Application.Context;
using CaptainTrackBackend.Domain.Entities.Common;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace CaptainTrackBackend.Persistence.Repositories
{
    public class RepositoryAsync<T> : IRepositoryAsync<T> where T : AuditableEntity, new()
    {
        private readonly ApplicationDbContext _context;
        
        public RepositoryAsync(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<T> AddAsync(T entity)
        {
            await _context.Set<T>().AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<bool> ExistsAsync(Expression<Func<T, bool>> expression)
        {
            return await _context.Set<T>().AnyAsync(expression);
        }

        public async Task<T> GetAsync(Expression<Func<T, bool>> expression)
        {
            var ans = await _context.Set<T>().FirstOrDefaultAsync(expression);
            return ans;
        }

        public async Task<IList<T>> GetAllAsync()
        {
            return await _context.Set<T>().ToListAsync();
        }

        public Task<List<T>> GetAllByIdsAsync(List<Guid> ids)
        {
            throw new NotImplementedException();
        }

        public async Task<IList<T>> GetAllByExpression(Expression<Func<T, bool>> expression)
        {
            var get = await _context.Set<T>().Where(expression).ToListAsync();
            return get;
        }

        public async Task<T> UpdateAsync(T entity)
        {
            _context.Set<T>().Update(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

    }
}
