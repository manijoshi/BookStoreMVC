using BookStore.Domain.Entities;

namespace BookStore.Domain.ViewModels
{
    public class OrderViewModel {
		public OrderHeader OrderHeader { get; set; }
        public IEnumerable<OrderDetail> OrderDetail { get; set; }
    }
}
