using System.Linq.Expressions;

namespace BookStore.Domain.Interfaces.Repository
{
    public interface IRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetAllAsync(Expression<Func<T,bool>>? expression=null,string? includeProdperties=null);
        Task<T> GetByIdAsync(Expression<Func<T, bool>>? filter, string? includeProdperties);
        Task AddAsync(T entity);
        Task<T> UpdateAsync(T entity);
        Task DeleteAsync(T entity);
        Task<bool> ExistsAsync(int id);
    }
}
