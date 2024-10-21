using BookStore.Application.Services.Interfaces;
using BookStore.Domain.Entities;
using BookStore.Domain.Interfaces;
using BookStore.Domain.Interfaces.Repository;
using BookStore.Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BookStore.Application.Services.Implementation
{
    public class CartService : ICartService
    {
        public readonly IUnitOfWork _unitOfWork;
        public CartService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task Details(Cart shoppingCart,string userId)
        {
            var cartFromDb = await this.GetCartById(u => u.ApplicationUserId == userId &&
            u.ProductId == shoppingCart.ProductId);

            if (cartFromDb != null)
            {
                //shopping cart exists
                cartFromDb.Count += shoppingCart.Count;
                await this.UpdateCart(cartFromDb);
                await _unitOfWork.Save();
            }
            else
            {
                //add cart record
                //_context.Carts.Add(shoppingCart);
                try
                {
                    await _unitOfWork.Cart.AddAsync(shoppingCart);
                }
                catch(Exception ex)
                {

                }
                await _unitOfWork.Save();
                //HttpContext.Session.SetInt32(SD.SessionCart,
                //_unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == userId).Count());
            }
        }

        private async Task<IEnumerable<Cart>> GetAllCarts(Expression<Func<Cart,bool>> filter)
        {
            return await _unitOfWork.Cart.GetAllAsync(filter);
        }
        private async Task<Cart> GetCartById(Expression<Func<Cart, bool>> filter)
        {
            return await _unitOfWork.Cart.GetByIdAsync(filter,null);
        }

        private async Task UpdateCart(Cart cart)
        {
            await _unitOfWork.Cart.UpdateAsync(cart);
        }
    }
}
