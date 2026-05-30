using AeroManage.BookingManagement.Application.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Stripe;
using Stripe.V2.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AeroManage.BookingManagement.Application.Services.Implementation
{
    public class StripePaymentService : IStripePaymentService
    {
        private readonly ILogger<StripePaymentService> _logger;
        private readonly IConfiguration _configuration;
        private readonly PaymentIntentService _paymentIntentService;
        private readonly RefundService _refundService;

        public StripePaymentService(
            ILogger<StripePaymentService> logger,
            IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;

            // Set Stripe API key
            StripeConfiguration.ApiKey = configuration["Stripe:SecretKey"];

            _paymentIntentService = new PaymentIntentService();
            _refundService = new RefundService();
        }

        public async Task<PaymentIntent> ConfirmPaymentIntentAsync(string paymentIntentId)
        {
            try
            {
                var options = new PaymentIntentConfirmOptions
                {
                    ReturnUrl = _configuration["Stripe:ReturnUrl"]
                };
                var intent = await _paymentIntentService.ConfirmAsync(paymentIntentId, options);

                _logger.LogInformation($"Confirmed PaymentIntent {paymentIntentId}, status: {intent.Status}");

                return intent;
            }
            catch (StripeException ex)
            {
                _logger.LogError(ex, $"Stripe error confirming payment intent {paymentIntentId}");
                throw new Exception($"Payment confirmation failed: {ex.Message}", ex);
            }
        }

        public async Task<PaymentIntent> CreatePaymentIntentAsync(int bookingId, decimal amount, string currency, Dictionary<string, string> metadata)
        {
            try
            {
                var option = new PaymentIntentCreateOptions
                {
                    Amount = (long)(amount * 100),
                    Currency = currency.ToLower(),
                  /*  AutomaticPaymentMethods = new PaymentIntentAutomaticPaymentMethodsOptions
                    {
                        Enabled = true,
                    },*/
                    Metadata = metadata ?? new Dictionary<string, string>
                    {
                        { "booking_id", bookingId.ToString() }
                    },
                    Description = $"Booking #{bookingId}",
                    ReceiptEmail = metadata?.GetValueOrDefault("email"),
                };
                var intent = await _paymentIntentService.CreateAsync(option);
                _logger.LogInformation($"Created PaymentIntent {intent.Id} for booking {bookingId}, amount: {amount} {currency}");
                return intent;
            }
            catch (StripeException ex)
            {
                _logger.LogError(ex, $"Stripe error creating payment intent for booking {bookingId}");
                throw new Exception($"Payment failed: {ex.Message}", ex);
            }
        }

        public async Task<Refund> CreateRefundAsync(string paymentIntentId, decimal? amount = null, string reason = null)
        {
            try
            {
                var options = new RefundCreateOptions
                {
                    PaymentIntent = paymentIntentId,
                    Reason = reason switch
                    {
                        "duplicate" => "duplicate",
                        "fraudulent" => "fraudulent",
                        _ => "requested_by_customer"
                    }
                };

                if (amount.HasValue)
                {
                    options.Amount = (long)(amount.Value * 100);
                }

                var refund = await _refundService.CreateAsync(options);

                _logger.LogInformation($"Created refund {refund.Id} for PaymentIntent {paymentIntentId}, amount: {refund.Amount / 100m}");

                return refund;
            }
            catch (StripeException ex)
            {
                _logger.LogError(ex, $"Stripe error creating refund for {paymentIntentId}");
                throw new Exception($"Refund failed: {ex.Message}", ex);
            }
        }

        public async Task<PaymentIntent> GetPaymentIntentAsync(string paymentIntentId)
        {
            try
            {
                var intent = await _paymentIntentService.GetAsync(paymentIntentId);
                return intent;
            }
            catch (StripeException ex)
            {
                _logger.LogError(ex, $"Stripe error retrieving payment intent {paymentIntentId}");
                throw new Exception($"Failed to retrieve payment: {ex.Message}", ex);
            }
        }

        public async Task<bool> HandleWebhookAsync(string payload, string signature)
        {
            try
            {
                var webhookSecret = _configuration["Stripe:WebhookSecret"];

                var stripeEvent = EventUtility.ConstructEvent(
                    payload,
                    signature,
                    webhookSecret
                );

                _logger.LogInformation($"Received Stripe webhook: {stripeEvent.Type}");

                // Handle the event
                switch (stripeEvent.Type)
                {
                    case "succeded":
                        var paymentIntent = stripeEvent.Data.Object as PaymentIntent;
                        _logger.LogInformation($"PaymentIntent {paymentIntent.Id} succeeded");
                        // This will be handled by the webhook controller
                        break;

                    case "failed":
                        var failedIntent = stripeEvent.Data.Object as PaymentIntent;
                        _logger.LogWarning($"PaymentIntent {failedIntent.Id} failed");
                        break;

                    case "refund":
                        var charge = stripeEvent.Data.Object as Charge;
                        _logger.LogInformation($"Charge {charge.Id} refunded");
                        break;

                    default:
                        _logger.LogInformation($"Unhandled event type: {stripeEvent.Type}");
                        break;
                }

                return true;
            }
            catch (StripeException ex)
            {
                _logger.LogError(ex, "Error processing Stripe webhook");
                return false;
            }
        }
    }
}
