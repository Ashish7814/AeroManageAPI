using AeroManage.BookingManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AeroManage.BookingManagement.Domain.Interfaces
{
    public interface IPaymentRepository
    {
        Task<Payment> CreatePaymentAsync(Payment payment);
        Task<Payment> GetPaymentByIdAsync(int paymentId);
        Task<Payment> GetPaymentByBookingIdAsync(int bookingId);
        Task<Payment> GetPaymentByIntentIdAsync(string paymentIntentId);
        Task<bool> UpdatePaymentStatusAsync(int paymentId, string status, DateTime? paymentDate = null);
        Task<bool> ProcessRefundAsync(int paymentId, decimal refundAmount, DateTime refundDate, string stripeRefundId, string status);
        Task<(int RefundId, string RefundReference)> CreateRefundRequestAsync(int bookingId, int paymentId, decimal refundAmount, decimal cancellationFee,
         string reason, string bankAccount, string bankName, int requestedBy);
        Task<BookingPricing> CreatePricingAsync(BookingPricing pricing, IDbConnection connection, IDbTransaction transaction, CancellationToken cancellationToken = default);
        Task<decimal> CalculateCancellationFeeAsync(int bookingId);
    }
}
