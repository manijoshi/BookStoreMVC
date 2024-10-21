using BookStore.Application.Utilities;
using BookStore.Domain.Entities;
using BookStore.Domain.Interfaces;
using BookStore.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;

namespace BookStore.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWork _unitOfWork;
        public HomeController(ILogger<HomeController> logger,IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }
        public IActionResult Details(int productId)
        {
            var item = _unitOfWork.Product.GetByIdAsync(x => x.Id == productId, includeProdperties: "Category").Result;
            Cart cart = new()
            {
                Product = item,
                Count = 1,
                ProductId = productId
            };
            return View(cart);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Details(Cart shoppingCart)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity!;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier)!.Value;
            shoppingCart.ApplicationUserId = userId;

            Cart cartFromDb = await _unitOfWork.Cart.GetByIdAsync(u => u.ApplicationUserId == userId &&
            u.ProductId == shoppingCart.ProductId, null);

            if (cartFromDb != null)
            {
                //shopping cart exists
                cartFromDb.Count += shoppingCart.Count;
                await _unitOfWork.Cart.UpdateAsync(cartFromDb);
                await _unitOfWork.Save();
            }
            else
            {
                //add cart record
                await _unitOfWork.Cart.AddAsync(shoppingCart);
                await _unitOfWork.Save();
                var count = await _unitOfWork.Cart.GetAllAsync(u => u.ApplicationUserId == userId);
                HttpContext.Session.SetInt32(SD.SessionCart, count.Count());
            }
            TempData["success"] = "Cart updated successfully";
            return RedirectToAction(nameof(Index), "Product");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
