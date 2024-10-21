using BookStore.Application.Services.Interfaces;
using BookStore.Domain.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BookStore.Web.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;
        public ProductController(IProductService productService, ICategoryService categoryService)
        {
            _productService = productService;
            _categoryService = categoryService;
        }
        public async Task<IActionResult> Index()
        {
            var products = await _productService.GetProducts();
            return View(products);
        }
        public async Task<IActionResult> Create()
        {
            var categories = await _categoryService.GetCategories();
            ProductViewModel productVM = new()
            {

                CategoryList = categories.Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                })
            };
            return View(productVM);
        }

        [HttpPost]
        public async Task<IActionResult> Create(ProductViewModel productVM,IFormFile img)
        {
            if (ModelState.IsValid)
            {
                await _productService.AddProduct(productVM,img);
                TempData["success"] = "Product created/updated successfully";
                return RedirectToAction("Index");
            }
            return View();
        }

        public async Task<IActionResult> Update(int itemId)
        {

            var categories = await _categoryService.GetCategories();
            var productDetail = await _productService.GetProductById(itemId);
            ProductViewModel productVM = new()
            {

                CategoryList = categories.Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                }),
                Product = productDetail
            };
            return View(productVM);
        }

        [HttpPost]
        public async Task<IActionResult> Update(ProductViewModel productViewModel,IFormFile img)
        {
            ModelState.Remove("img");
            if (ModelState.IsValid)
            {
                await _productService.UpdateProduct(productViewModel,img);
            }
            TempData["success"] = "Product updated successfully";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int itemId)
        {
            await _productService.DeleteProduct(itemId);
            TempData["success"] = "Product deleted successfully";
            return RedirectToAction(nameof(Index));
        }
    }
}
