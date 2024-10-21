using BookStore.Domain.Entities;
using BookStore.Domain.Interfaces.Repository;
using BookStore.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BookStore.Infrastructure.Repository
{
    public class CartRepository:Repository<Cart>,ICartRepository
    {
        public CartRepository(ApplicationDbContext context) : base(context)
        {

        }
        public void RemoveRange(IEnumerable<Cart> entity)
        {
            _context.RemoveRange(entity);
        }
    }
}
