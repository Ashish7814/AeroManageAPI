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
    public class TicketRepository : ITicketRepository
    {
        private readonly string _connectionString;
        private readonly IBookingRepository _bookingRepository;

        public TicketRepository(IConfiguration configuration, IBookingRepository bookingRepository)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
            _bookingRepository = bookingRepository;
        }

        private IDbConnection CreateConnection()
        {
            return new SqlConnection(_connectionString);
        }
        public async Task<(int BoardingPassId, string BoardingPassNumber, string QRCode)> GenerateBoardingPassAsync(
            int bookingPassengerId, string gate, DateTime boardingTime)
        {
            using var connection = CreateConnection();
            var result = await connection.QueryFirstOrDefaultAsync<dynamic>(
                "sp_GenerateBoardingPass",
                new
                {
                    BookingPassengerId = bookingPassengerId,
                    Gate = gate,
                    BoardingTime = boardingTime
                },
                commandType: CommandType.StoredProcedure
            );

            return (result.BoardingPassId, result.BoardingPassNumber, result.QRCode);
        }

        public async Task<PNRDetails> GetPNRDetailsAsync(string pnr)
        {
            using var connection = CreateConnection();

            var bookingSummary = await _bookingRepository.GetBookingSummaryAsync(
                (await connection.QueryFirstAsync<int>(
                    "SELECT BookingId FROM Bookings WHERE PNR = @PNR",
                    new { PNR = pnr }
                ))
            );

            var pnrDetails = new PNRDetails
            {
                PNR = pnr,
                BookingReference = bookingSummary.BookingReference,
                Booking = new Booking
                {
                    BookingId = bookingSummary.BookingId,
                    BookingReference = bookingSummary.BookingReference,
                    PNR = pnr,
                    BookingStatus = bookingSummary.BookingStatus,
                    TotalAmount = bookingSummary.TotalAmount
                }
            };

            return pnrDetails;
        }

        // ==================== EMAIL NOTIFICATIONS ====================

        public async Task<int> LogEmailNotificationAsync(int bookingId, string emailType,
            string recipient, string subject, string status)
        {
            using var connection = CreateConnection();
            var result = await connection.ExecuteScalarAsync<int>(
                "sp_LogEmailNotification",
                new
                {
                    BookingId = bookingId,
                    EmailType = emailType,
                    RecipientEmail = recipient,
                    Subject = subject,
                    Status = status
                },
                commandType: CommandType.StoredProcedure
            );

            return result;
        }

        public async Task<bool> UpdateEmailStatusAsync(int notificationId, string status, string errorMessage)
        {
            using var connection = CreateConnection();
            await connection.ExecuteAsync(
                "sp_UpdateEmailStatus",
                new
                {
                    NotificationId = notificationId,
                    Status = status,
                    ErrorMessage = errorMessage
                },
                commandType: CommandType.StoredProcedure
            );

            return true;
        }
    }

}

