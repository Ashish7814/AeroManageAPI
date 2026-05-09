using AeroManage.BookingManagement.Domain.Entities;
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
        private readonly string _connectionString;
        public PaymentRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        private IDbConnection CreateConnection()
        {
            return new SqlConnection(_connectionString);
        }

        public async Task<Payment> CreatePaymentAsync(Payment payment)
        {
            using var connection = CreateConnection();

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

            var paymentId = await connection.ExecuteScalarAsync<int>(sql, payment);
            payment.PaymentId = paymentId;

            return payment;
        }

        public async Task<Payment> GetPaymentByIdAsync(int paymentId)
        {
            using var connection = CreateConnection();

            var sql = "SELECT * FROM Payments WHERE PaymentId = @PaymentId";

            return await connection.QueryFirstOrDefaultAsync<Payment>(sql, new { PaymentId = paymentId });
        }
        public async Task<Payment> GetPaymentByBookingIdAsync(int bookingId)
        {
            using var connection = CreateConnection();

            var sql = @"
                SELECT TOP 1 * 
                FROM Payments 
                WHERE BookingId = @BookingId 
                ORDER BY CreatedAt DESC";

            return await connection.QueryFirstOrDefaultAsync<Payment>(sql, new { BookingId = bookingId });
        }

        public async Task<Payment> GetPaymentByIntentIdAsync(string paymentIntentId)
        {
            using var connection = CreateConnection();

            var sql = "SELECT * FROM Payments WHERE PaymentIntentId = @PaymentIntentId";

            return await connection.QueryFirstOrDefaultAsync<Payment>(sql, new { PaymentIntentId = paymentIntentId });
        }

        public async Task<bool> UpdatePaymentStatusAsync(int paymentId, string status, DateTime? paymentDate)
        {
            using var connection = CreateConnection();

            var sql = @"
                UPDATE Payments
                SET PaymentStatus = @Status,
                    PaymentDate = @PaymentDate
                WHERE PaymentId = @PaymentId";

            var rowsAffected = await connection.ExecuteAsync(sql, new
            {
                PaymentId = paymentId,
                Status = status,
                PaymentDate = paymentDate
            });

            return rowsAffected > 0;
        }

        // ==================== PROCESS REFUND ====================

        public async Task<bool> ProcessRefundAsync(int paymentId, decimal refundAmount, DateTime refundDate)
        {
            using var connection = CreateConnection();

            var sql = @"
                UPDATE Payments
                SET RefundAmount = @RefundAmount,
                    RefundDate = @RefundDate
                WHERE PaymentId = @PaymentId";

            var rowsAffected = await connection.ExecuteAsync(sql, new
            {
                PaymentId = paymentId,
                RefundAmount = refundAmount,
                RefundDate = refundDate
            });

            return rowsAffected > 0;
        }

    }
}
