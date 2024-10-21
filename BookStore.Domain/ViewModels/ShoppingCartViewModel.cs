using BookStore.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookStore.Domain.ViewModels {
	public class ShoppingCartViewModel {
		public IEnumerable<Cart> ShoppingCartList { get; set; }
        public OrderHeader OrderHeader { get; set; }
     
    }
}
