using BookStore.Domain.Interfaces.Repository;
using BookStore.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;


namespace BookStore.Infrastructure.Repository
{
    public class Repository<T> : IRepository<T> where T : class
    {
        protected readonly ApplicationDbContext _context;
        protected readonly DbSet<T> _dbSet;

        public Repository(ApplicationDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }

        public async Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>>? filter = null, string? includeProdperties = null)
        {
            IQueryable<T> query = FilterQuery(filter, includeProdperties);
            return await query.ToListAsync();
        }

        private IQueryable<T> FilterQuery(Expression<Func<T, bool>>? filter, string? includeProdperties)
        {
            IQueryable<T> query = _dbSet;
            if (filter != null)
            {
                query = query.Where(filter);
            }
            if (!(string.IsNullOrEmpty(includeProdperties)))
            {
                foreach (var prop in includeProdperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(prop);
                }
            }

            return query;
        }

        public virtual async Task<T> GetByIdAsync(Expression<Func<T, bool>>? filter, string? includeProdperties)
        {
            IQueryable<T> query = FilterQuery(filter, includeProdperties);
            return await query.FirstOrDefaultAsync();
        }

        public async Task AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
        }

        public async Task<T> UpdateAsync(T entity)
        {
            //var keyValues = _context.Model.FindEntityType(typeof(T))
            //    .FindPrimaryKey()
            //    .Properties
            //    .Select(p => typeof(T).GetProperty(p.Name).GetValue(entity))
            //    .ToArray();
            //var existingEntity = await _dbSet.FindAsync(keyValues);
            //if(existingEntity != null)
            //    _dbSet.Entry(existingEntity).CurrentValues.SetValues(entity);
            _dbSet.Update(entity);
            return entity;
        }

        public async Task DeleteAsync(T entity)
        {
            _dbSet.Remove(entity);
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _dbSet.FindAsync(id) != null;
        }
    }
}
