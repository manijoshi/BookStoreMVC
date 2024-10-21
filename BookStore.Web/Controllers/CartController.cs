using BookStore.Application.Services.Interfaces;
using BookStore.Application.Utilities;
using BookStore.Domain.Entities;
using BookStore.Domain.Interfaces;
using BookStore.Domain.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Stripe.Checkout;
using Stripe.Climate;
using System.Security.Claims;

namespace BookStore.Web.Controllers
{
    public class CartController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPaymentService _paymentService;
        [BindProperty]
        public ShoppingCartViewModel ShoppingCartVM { get; set; }
        public CartController(IUnitOfWork unitOfWork, IPaymentService paymentService)
        {
            _unitOfWork = unitOfWork;
            _paymentService = paymentService;
            ShoppingCartVM = new ShoppingCartViewModel();
        }
        public async Task<IActionResult> Index()
        {

            var claimsIdentity = (ClaimsIdentity)User.Identity!;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier)!.Value;

            ShoppingCartVM = new()
            {
                ShoppingCartList = await _unitOfWork.Cart.GetAllAsync(u => u.ApplicationUserId == userId,
                includeProdperties: "Product"),
                OrderHeader = new()
            };

            foreach (var cart in ShoppingCartVM.ShoppingCartList)
            {
                cart.Price = GetPriceBasedOnQuantity(cart);
                ShoppingCartVM.OrderHeader.OrderTotal += (cart.Price * cart.Count);
            }

            return View(ShoppingCartVM);
        }
        public async Task<IActionResult> Plus(int cartId)
        {
            var cartFromDb = await _unitOfWork.Cart.GetByIdAsync(u => u.Id == cartId,null);
            cartFromDb.Count += 1;
            await _unitOfWork.Cart.UpdateAsync(cartFromDb);
            await _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Minus(int cartId)
        {
            var cartFromDb = await _unitOfWork.Cart.GetByIdAsync(u => u.Id == cartId,null);
            if (cartFromDb.Count <= 1)
            {
                //remove that from cart
                var items = await _unitOfWork.Cart
                    .GetAllAsync(u => u.ApplicationUserId == cartFromDb.ApplicationUserId);
                await _unitOfWork.Cart.DeleteAsync(cartFromDb);
                HttpContext.Session.SetInt32(SD.SessionCart, items.Count() - 1);
            }
            else
            {
                cartFromDb.Count -= 1;
                await _unitOfWork.Cart.UpdateAsync(cartFromDb);
            }

            await _unitOfWork.Save();
            return RedirectToAction(nameof(Index),"Product");
        }

        public async Task<IActionResult> Remove(int cartId)
        {
            var cartFromDb = await _unitOfWork.Cart.GetByIdAsync(u => u.Id == cartId, null);

            await _unitOfWork.Cart.DeleteAsync(cartFromDb);

            var items = await _unitOfWork.Cart
              .GetAllAsync(u => u.ApplicationUserId == cartFromDb.ApplicationUserId);
            HttpContext.Session.SetInt32(SD.SessionCart, items.Count() - 1);

            await _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Summary()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity!;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier)!.Value;

            ShoppingCartVM = new()
            {
                ShoppingCartList = await _unitOfWork.Cart.GetAllAsync(u => u.ApplicationUserId == userId,
                includeProdperties: "Product"),
                OrderHeader = new()
            };

            ShoppingCartVM.OrderHeader.ApplicationUser = await _unitOfWork.ApplicationUser.GetByIdAsync(u => u.Id == userId,null);

            ShoppingCartVM.OrderHeader.Name = ShoppingCartVM.OrderHeader.ApplicationUser.Name;
            ShoppingCartVM.OrderHeader.PhoneNumber = ShoppingCartVM.OrderHeader.ApplicationUser.PhoneNumber;
            ShoppingCartVM.OrderHeader.StreetAddress = ShoppingCartVM.OrderHeader.ApplicationUser.StreetAddress;
            ShoppingCartVM.OrderHeader.City = ShoppingCartVM.OrderHeader.ApplicationUser.City;
            ShoppingCartVM.OrderHeader.State = ShoppingCartVM.OrderHeader.ApplicationUser.State;
            ShoppingCartVM.OrderHeader.PostalCode = ShoppingCartVM.OrderHeader.ApplicationUser.PostalCode;

            foreach (var cart in ShoppingCartVM.ShoppingCartList)
            {
                cart.Price = GetPriceBasedOnQuantity(cart);
                ShoppingCartVM.OrderHeader.OrderTotal += (cart.Price * cart.Count);
            }
            return View(ShoppingCartVM);
        }

        [HttpPost]
        [ActionName("Summary")]
        public async Task<IActionResult> SummaryPOST()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity!;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier)!.Value;

            ShoppingCartVM.ShoppingCartList = await _unitOfWork.Cart.GetAllAsync(u => u.ApplicationUserId == userId,
                includeProdperties: "Product");

            ShoppingCartVM.OrderHeader.OrderDate = System.DateTime.Now;
            ShoppingCartVM.OrderHeader.ApplicationUserId = userId;

            ApplicationUser applicationUser = await _unitOfWork.ApplicationUser.GetByIdAsync(u => u.Id == userId, null);


            foreach (var cart in ShoppingCartVM.ShoppingCartList)
            {
                cart.Price = GetPriceBasedOnQuantity(cart);
                ShoppingCartVM.OrderHeader.OrderTotal += (cart.Price * cart.Count);
            }

            
            //it is a regular customer 
            ShoppingCartVM.OrderHeader.PaymentStatus = SD.PaymentStatusPending;
            ShoppingCartVM.OrderHeader.OrderStatus = SD.StatusPending;
            
            await _unitOfWork.OrderHeader.AddAsync(ShoppingCartVM.OrderHeader);
            await _unitOfWork.Save();

            foreach (var cart in ShoppingCartVM.ShoppingCartList)
            {
                OrderDetail orderDetail = new()
                {
                    ProductId = cart.ProductId,
                    OrderHeaderId = ShoppingCartVM.OrderHeader.Id,
                    Price = cart.Price,
                    Count = cart.Count
                };
                await _unitOfWork.OrderDetail.AddAsync(orderDetail);
                await _unitOfWork.Save();
            }

            //it is a regular customer account and we need to capture payment
            //stripe logic
            var domain = Request.Scheme + "://" + Request.Host.Value + "/";
            var options = _paymentService.CreateStripeSessionOptions(ShoppingCartVM, domain);
            var session = _paymentService.CreateStripeSession(options);
            _unitOfWork.OrderHeader.UpdateStripePaymentID(ShoppingCartVM.OrderHeader.Id, session.Id, session.PaymentIntentId);
            await _unitOfWork.Save();
			Response.Headers.Add("Location", session.Url);
			return new StatusCodeResult(303);

            //return RedirectToAction(nameof(OrderConfirmation), new { id = ShoppingCartVM.OrderHeader.Id });
        }

        public async Task<IActionResult> OrderConfirmation(int id)
        {

            OrderHeader orderHeader = await _unitOfWork.OrderHeader.GetByIdAsync(u => u.Id == id, includeProdperties: "ApplicationUser");
            if (orderHeader.PaymentStatus == SD.PaymentStatusPending)
            {
                var service = new SessionService();
                Session session = service.Get(orderHeader.SessionId);

                if (session.PaymentStatus.ToLower() == "paid")
                {
                    _unitOfWork.OrderHeader.UpdateStripePaymentID(id, session.Id, session.PaymentIntentId);
                    _unitOfWork.OrderHeader.UpdateStatus(id, SD.StatusApproved, SD.PaymentStatusApproved);
                    await _unitOfWork.Save();
                }
                HttpContext.Session.Clear();

            }

            //_emailSender.SendEmailAsync(orderHeader.ApplicationUser.Email, "New Order - Bulky Book",
            //    $"<p>New Order Created - {orderHeader.Id}</p>");

            List<Cart> shoppingCarts = _unitOfWork.Cart
                .GetAllAsync(u => u.ApplicationUserId == orderHeader.ApplicationUserId).Result.ToList();

            _unitOfWork.Cart.RemoveRange(shoppingCarts);
            await _unitOfWork.Save();

            return View(id);
        }
        private double GetPriceBasedOnQuantity(Cart shoppingCart)
        {
            return shoppingCart.Product.Price;
        }
    }
}
