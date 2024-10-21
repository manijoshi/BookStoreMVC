using BookStore.Domain.Interfaces;
using BookStore.Domain.Interfaces.Repository;
using BookStore.Infrastructure.Data;
using BookStore.Infrastructure.Repository;

namespace BookStore.Infrastructure
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        public IProductRepository Product { get; private set; }
        public ICategoryRepository Category { get; private set; }
        public ICartRepository Cart { get; private set; } 
        public IOrderHeaderRepository OrderHeader { get; private set; }
        public IApplicationUserRepository ApplicationUser { get; private set; }
        public IOrderDetailRepository OrderDetail { get; private set; }
        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
            Product = new ProductRepository(_context);
            Category = new CategoryRepository(_context);
            Cart = new CartRepository(_context);
            OrderHeader = new OrderHeaderRepository(_context);
            ApplicationUser = new ApplicationUserRepository(_context);
            OrderDetail = new OrderDetailRepository(_context);
        }

        public async Task Save()
        {
            await _context.SaveChangesAsync();
        }
    }
}
