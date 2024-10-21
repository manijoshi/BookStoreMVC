using BookStore.Domain.Entities;
using BookStore.Domain.Interfaces.Repository;
using BookStore.Infrastructure.Data;

namespace BookStore.Infrastructure.Repository
{
    public class ProductRepository : Repository<Product>,IProductRepository
    {
        public ProductRepository(ApplicationDbContext context):base(context)
        {
            
        }
    }
}
