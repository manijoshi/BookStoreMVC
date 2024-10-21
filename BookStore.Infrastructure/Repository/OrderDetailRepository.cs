using BookStore.Domain.Entities;
using BookStore.Domain.Interfaces.Repository;
using BookStore.Infrastructure.Data;

namespace BookStore.Infrastructure.Repository
{
    public class OrderDetailRepository:Repository<OrderDetail>,IOrderDetailRepository
    {
        public OrderDetailRepository(ApplicationDbContext context):base(context)
        {           
        }
    }
}
