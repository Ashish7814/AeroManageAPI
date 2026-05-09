using Stripe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AeroManage.BookingManagement.Application.Services.Interfaces
{
    public interface IStripePaymentService
    {
        Task<PaymentIntent> CreatePaymentIntentAsync(int bookingId, decimal amount, string currency, Dictionary<string, string> metadata);
        Task<PaymentIntent> ConfirmPaymentIntentAsync(string paymentIntentId);
        Task<PaymentIntent> GetPaymentIntentAsync(string paymentIntentId);
        Task<Refund> CreateRefundAsync(string paymentIntentId, decimal? amount = null, string reason = null);
        Task<bool> HandleWebhookAsync(string payload, string signature);
    }
}
