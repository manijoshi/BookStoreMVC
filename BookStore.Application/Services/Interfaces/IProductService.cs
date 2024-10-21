using BookStore.Domain.Entities;
using BookStore.Domain.ViewModels;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookStore.Application.Services.Interfaces
{
    public interface IProductService
    {
        Task<IEnumerable<Product>> GetProducts();
        Task<Product> GetProductById(int Id,string? includeProperties = null);
        Task AddProduct(ProductViewModel product, IFormFile img);
        Task DeleteProduct(int productId);
        Task UpdateProduct(ProductViewModel productVM, IFormFile img);

    }
}
