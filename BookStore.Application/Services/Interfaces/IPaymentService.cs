using BookStore.Domain.ViewModels;
using Stripe.Checkout;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookStore.Application.Services.Interfaces
{
    public interface IPaymentService
    {
        SessionCreateOptions CreateStripeSessionOptions(ShoppingCartViewModel shoppingCartViewModel, string domain);
        Session CreateStripeSession(SessionCreateOptions options);

    }
}
