using BookStore.Application.Services.Interfaces;
using BookStore.Domain.Entities;
using BookStore.Domain.Interfaces;
using BookStore.Domain.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace BookStore.Application.Services.Implementation
{
    public class ProductService : IProductService
    {
        private readonly IUnitOfWork _unitOfWork;

        private readonly IWebHostEnvironment _webHostEnvironment;
        public ProductService(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task AddProduct(ProductViewModel productVM, IFormFile img)
        {
            if (img is not null)
            {
                SaveImage(productVM, img);
            }

            await _unitOfWork.Product.AddAsync(productVM.Product);
            await _unitOfWork.Save();
        }

        private void SaveImage(ProductViewModel productVM, IFormFile img)
        {
            string fileName = Guid.NewGuid().ToString() + Path.GetExtension(img.FileName);
            string productPath = @"images\products";
            var finalpath = Path.Combine(_webHostEnvironment.WebRootPath, productPath);
            using (var fileStream = new FileStream(Path.Combine(finalpath, fileName), FileMode.Create))
            {
                img.CopyTo(fileStream);
            }
            productVM.Product.ProductImage = @"\" + productPath + @"\" + fileName;
        }

        public async Task DeleteProduct(int productId)
        {
            try
            {
                var product = await this.GetProductById(productId);
                await _unitOfWork.Product.DeleteAsync(product);
                await _unitOfWork.Save();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }            
        }

        public async Task<Product> GetProductById(int productId, string? includeProperties = null)
        {
            var product = await _unitOfWork.Product.GetByIdAsync(u=>u.Id==productId,null);
            ArgumentNullException.ThrowIfNull(product);
            return product;
        }

        public async Task<IEnumerable<Product>> GetProducts()
        {
            return await _unitOfWork.Product.GetAllAsync();
        }

        public async Task UpdateProduct(ProductViewModel productVM, IFormFile img)
        {
            if (img is not null)
            {
                SaveImage(productVM, img);
            }
            //var product = await this.GetProductById(productVM.Product.Id);
            await _unitOfWork.Product.UpdateAsync(productVM.Product);
            await this._unitOfWork.Save();

        }
    }
}
