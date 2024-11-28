using System;
    using Stripe;

namespace Application.Services
{

    public class PaymentService
    {
        private readonly string stripeSecretKey = "your_stripe_secret_key"; 

        public PaymentService()
        {
            StripeConfiguration.ApiKey = stripeSecretKey;
        }

        public async Task<PaymentIntent> CreatePaymentIntentAsync(decimal amount, string currency = "usd")
        {
            var options = new PaymentIntentCreateOptions
            {
                Amount = (long)(amount * 100), 
                Currency = currency,
                PaymentMethodTypes = new List<string> { "card" }, 
            };

            var service = new PaymentIntentService();
            var paymentIntent = await service.CreateAsync(options);

            return paymentIntent;
        }

        public async Task<PaymentIntent> ConfirmPaymentAsync(string paymentIntentId, string paymentMethodId)
        {
            var options = new PaymentIntentConfirmOptions
            {
                PaymentMethod = paymentMethodId, 
            };

            var service = new PaymentIntentService();
            var paymentIntent = await service.ConfirmAsync(paymentIntentId, options);

            return paymentIntent;
        }
    }

}

