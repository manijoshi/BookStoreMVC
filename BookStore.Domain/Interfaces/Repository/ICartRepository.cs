using BookStore.Domain.Entities;
using System.Linq.Expressions;

namespace BookStore.Domain.Interfaces.Repository
{
    public interface ICartRepository:IRepository<Cart>
    {
        void RemoveRange(IEnumerable<Cart> entity);
    }
}
