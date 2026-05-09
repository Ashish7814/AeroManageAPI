using AeroManage.BookingManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AeroManage.BookingManagement.Infrastructure.Repositories.Interfaces
{
    public interface IPaymentRepository
    {
        Task<Payment> CreatePaymentAsync(Payment payment);
        Task<Payment> GetPaymentByIdAsync(int paymentId);
        Task<Payment> GetPaymentByBookingIdAsync(int bookingId);
        Task<Payment> GetPaymentByIntentIdAsync(string paymentIntentId);
        Task<bool> UpdatePaymentStatusAsync(int paymentId, string status, DateTime? paymentDate = null);
        Task<bool> ProcessRefundAsync(int paymentId, decimal refundAmount, DateTime refundDate);
    }
}
