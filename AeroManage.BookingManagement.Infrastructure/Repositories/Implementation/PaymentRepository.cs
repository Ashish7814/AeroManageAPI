using AeroManage.BookingManagement.Domain.Entities;
using AeroManage.BookingManagement.Domain.Interfaces;
using AeroManage.BookingManagement.Infrastructure.Repositories.Interfaces;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AeroManage.BookingManagement.Infrastructure.Repositories.Implementation
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly IDapperUnitOfWork _unitOfWork;
        public PaymentRepository(IDapperUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<Payment> CreatePaymentAsync(Payment payment)
        {
            var sql = @"
                INSERT INTO Payments (
                    BookingId, PaymentReference, PaymentIntentId, PaymentMethodId,
                    Amount, Currency, PaymentStatus, PaymentMethod, Metadata
                )
                VALUES (
                    @BookingId, @PaymentReference, @PaymentIntentId, @PaymentMethodId,
                    @Amount, @Currency, @PaymentStatus, @PaymentMethod, @Metadata
                );
                SELECT CAST(SCOPE_IDENTITY() as int);";

            var paymentId = await _unitOfWork.Connection.ExecuteScalarAsync<int>(sql, payment);
            payment.PaymentId = paymentId;

            return payment;
        }

        public async Task<Payment> GetPaymentByIdAsync(int paymentId)
        {
            var sql = "SELECT * FROM Payments WHERE PaymentId = @PaymentId";

            return await _unitOfWork.Connection.QueryFirstOrDefaultAsync<Payment>(sql, new { PaymentId = paymentId });
        }
        public async Task<Payment> GetPaymentByBookingIdAsync(int bookingId)
        {
            var sql = @"
                SELECT TOP 1 * 
                FROM Payments 
                WHERE BookingId = @BookingId 
                ORDER BY CreatedAt DESC";

            return await _unitOfWork.Connection.QueryFirstOrDefaultAsync<Payment>(sql, new { BookingId = bookingId });
        }

        public async Task<Payment> GetPaymentByIntentIdAsync(string paymentIntentId)
        {
            var sql = "SELECT * FROM Payments WHERE PaymentIntentId = @PaymentIntentId";

            return await _unitOfWork.Connection.QueryFirstOrDefaultAsync<Payment>(sql, new { PaymentIntentId = paymentIntentId });
        }

        public async Task<bool> UpdatePaymentStatusAsync(int paymentId, string status, DateTime? paymentDate)
        {
            var sql = @"
                UPDATE Payments
                SET PaymentStatus = @Status,
                    PaymentDate = @PaymentDate
                WHERE PaymentId = @PaymentId";

            var rowsAffected = await _unitOfWork.Connection.ExecuteAsync(sql, new
            {
                PaymentId = paymentId,
                Status = status,
                PaymentDate = paymentDate
            });

            return rowsAffected > 0;
        }

        // ==================== PROCESS REFUND ====================

        public async Task<bool> ProcessRefundAsync(int paymentId, decimal refundAmount, DateTime refundDate, string stripeRefundId, string status)
        {
            var sql = @"
                UPDATE Payments
                SET RefundAmount = @RefundAmount,
                    RefundDate = @RefundDate,
                    StripeRefundId = @StripeRefundId,
                    Status = @Status
                WHERE PaymentId = @PaymentId";

            var rowsAffected = await _unitOfWork.Connection.ExecuteAsync(sql, new
            {
                PaymentId = paymentId,
                RefundAmount = refundAmount,
                RefundDate = refundDate,
                StripeRefundId = stripeRefundId,
                Status = status
            });

            return rowsAffected > 0;
        }

        // ==================== REFUNDS ====================

        public async Task<(int RefundId, string RefundReference)> CreateRefundRequestAsync(
            int bookingId, int paymentId, decimal refundAmount, decimal cancellationFee,
            string reason, string bankAccount, string bankName, int requestedBy)
        {
            var result = await _unitOfWork.Connection.QueryFirstOrDefaultAsync<dynamic>(
                "sp_CreateRefundRequest",
                new
                {
                    BookingId = bookingId,
                    PaymentId = paymentId,
                    RefundAmount = refundAmount,
                    CancellationFee = cancellationFee,
                    RefundReason = reason,
                    BankAccountNumber = bankAccount,
                    BankName = bankName,
                    RequestedBy = requestedBy
                },
                commandType: CommandType.StoredProcedure
            );

            return (result.RefundId, result.RefundReference);
        }

        public async Task<BookingPricing> CreatePricingAsync(BookingPricing pricing, CancellationToken cancellationToken = default)
        {
            //var sql = @"INSERT INTO BookingPricing (...) VALUES (...)";
            //await con.ExecuteAsync(sql, pricing, tx);
            const string sql = @"
                INSERT INTO BookingPricing (
                    BookingId, TotalAmount, DiscountAmount, PromoCode, Currency, CreatedAt
                )
                VALUES (
                    @BookingId, @TotalAmount, @DiscountAmount, @PromoCode, @Currency, @CreatedAt
                );
                SELECT CAST(SCOPE_IDENTITY() AS INT);";

            var pricingId = await _unitOfWork.Connection.ExecuteScalarAsync<int>(
                new CommandDefinition(sql, pricing, 
                    cancellationToken: cancellationToken));

            pricing.PricingId = pricingId;
            return pricing;
        }

        public async Task<decimal> CalculateCancellationFeeAsync(int bookingId)
        {
            var result = await _unitOfWork.Connection.QueryFirstOrDefaultAsync<dynamic>(
                "sp_CalculateCancellationFee",
                new { BookingId = bookingId },
                commandType: CommandType.StoredProcedure
            );

            return result?.CancellationFee ?? 0;
        }

    }
}
