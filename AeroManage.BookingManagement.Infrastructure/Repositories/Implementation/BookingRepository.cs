using AeroManage.BookingManagement.Domain.Entities;
using AeroManage.BookingManagement.Infrastructure.Repositories.Interfaces;
using AeroManage.Shared.DTos;
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
    public class BookingRepository : IBookingRepository
    {
        private readonly string _connectionString;

        public BookingRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public IDbConnection CreateConnection()
        {
            return new SqlConnection(_connectionString);
        }

        public async Task<List<CalendarFare>> GetCalendarFaresAsync(
            int originId, int destId, DateTime start, DateTime end, string seatClass)
        {
            try
            {
                using var connection = CreateConnection();
                var results = await connection.QueryAsync<CalendarFare>(
                     "sp_GetCalendarFares",
                    new
                    {
                        OriginAirportId = originId,
                        DestinationAirportId = destId,
                        StartDate = start,
                        EndDate = end,
                        SeatClass = seatClass
                    },
                commandType: CommandType.StoredProcedure
                );
                return results.ToList();
            }
            catch(Exception ex)
            {
                throw;
            }
        }




        public async Task<FlightDetails> GetFlightDetailsAsync(int flightId)
        {
            try
            {
                using var connection = CreateConnection();
                using var multi = await connection.QueryMultipleAsync(
                "sp_GetFlightDetails",
                    new { FlightId = flightId },
                    commandType: CommandType.StoredProcedure
                );

                var flight = await multi.ReadFirstOrDefaultAsync<FlightDetails>();
                if (flight != null)
                {
                    flight.Layovers = (await multi.ReadAsync<RouteLayover>()).ToList();
                }

                return flight;
            }
            catch(Exception ex)
            {
                throw;
            }
        }

        public async Task<SeatAvailability> GetSeatAvailabilityAsync(int flightId, string seatClass)
        {
            try
            {
                using var connection = CreateConnection();
                var results = await connection.QueryAsync<dynamic>(
                    "sp_GetSeatAvailability",
                    new { FlightId = flightId, SeatClass = seatClass },
                    commandType: CommandType.StoredProcedure
                );

                var availability = new SeatAvailability();
                foreach (var row in results)
                {
                    if (row.SeatClass == "Economy")
                    {
                        availability.EconomyAvailable = row.AvailableSeats;
                    }
                    else if (row.SeatClass == "Business")
                    {
                        availability.BusinessAvailable = row.AvailableSeats;
                    }
                    else if (row.SeatClass == "FirstClass")
                    {
                        availability.FirstClassAvailable = row.AvailableSeats;
                    }
                    availability.AvailableSeats += (int)row.AvailableSeats;
                }

                return availability;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<List<Airline>> GetActiveAirlinesAsync()
        {
            try
            {
                using var connection = CreateConnection();
                var results = await connection.QueryAsync<Airline>(
                    "sp_GetActiveAirlines",
                    commandType: CommandType.StoredProcedure
                );

                return results.ToList();
            }
            catch(Exception ex)
            {
                throw;
            }
        }

        public async Task<bool> AddMealPreferenceAsync(int bookingPassengerId, string mealType, string instructions)
        {
            try
            {
                using var connection = CreateConnection();
                var result = await connection.ExecuteScalarAsync<int>(
                    "sp_AddMealPreference",
                    new
                    {
                        BookingPassengerId = bookingPassengerId,
                        MealType = mealType,
                        SpecialInstructions = instructions
                    },
                    commandType: CommandType.StoredProcedure
                );

                return result == 1;
            }
            catch(Exception ex)
            {
                throw;
            }
        }

        public async Task<bool> AddSpecialAssistanceAsync(int bookingPassengerId, string types, string details)
        {
            try
            {
                using var connection = CreateConnection();
                var result = await connection.ExecuteScalarAsync<int>(
                    "sp_AddSpecialAssistance",
                    new
                    {
                        BookingPassengerId = bookingPassengerId,
                        AssistanceTypes = string.Join(",", types),
                        Details = details
                    },
                    commandType: CommandType.StoredProcedure
                );

                return result == 1;
            }
            catch(Exception ex)
            {
                throw;
            }
        }

        public async Task<decimal> AddBookingAddonAsync(int bookingPassengerId, int extraBaggage,
            bool travelInsurance, bool priorityBoarding, bool loungeAccess)
        {
            try
            {
                using var connection = CreateConnection();
                var result = await connection.ExecuteScalarAsync<decimal>(
                    "sp_AddBookingAddons",
                    new
                    {
                        BookingPassengerId = bookingPassengerId,
                        ExtraBaggage = extraBaggage,
                        TravelInsurance = travelInsurance,
                        PriorityBoarding = priorityBoarding,
                        LoungeAccess = loungeAccess
                    },
                    commandType: CommandType.StoredProcedure
                );

                return result;
            }
            catch(Exception ex)
            {
                throw;
            }
        }

        public async Task<BookingSummary> GetBookingSummaryAsync(int bookingId)
        {
            using var connection = CreateConnection();
            using var multi = await connection.QueryMultipleAsync(
                "sp_GetBookingSummary",
                new { BookingId = bookingId },
                commandType: CommandType.StoredProcedure
            );

            // 1. Booking
            var booking = await multi.ReadFirstOrDefaultAsync<BookingSummary>();
            if (booking == null)
                return null;

            // 2. Flights
            booking.Flights = (await multi.ReadAsync<FlightSummary>()).ToList();

            // 3. Passengers
            booking.Passengers = (await multi.ReadAsync<PassengerSummary>()).ToList();

            // 4. Pricing
            booking.Pricing = await multi.ReadFirstOrDefaultAsync<PricingSummary>();

            return booking;
        }

        /*   public async Task<BookingSummary> GetBookingSummaryAsync(int bookingId)
           {
               try
               {
                   using var connection = CreateConnection();
                   using var multi = await connection.QueryMultipleAsync(
                       "sp_GetBookingSummary",
                       new { BookingId = bookingId },
                       commandType: CommandType.StoredProcedure
                   );

                   var summary = new BookingSummary();

                   // Booking details
                   var booking = await multi.ReadFirstOrDefaultAsync<dynamic>();
                   if (booking != null)
                   {
                       summary.BookingId = booking.BookingId;
                       summary.BookingReference = booking.BookingReference;
                       summary.PNR = booking.PNR;
                       summary.BookingStatus = booking.BookingStatus;
                       summary.TotalAmount = booking.TotalAmount;
                       summary.Currency = booking.Currency;
                       summary.BookingDate = booking.BookingDate;
                   }

                   // Flights
                   summary.Flights = (await multi.ReadAsync<FlightSummary>()).ToList();

                   // Passengers
                   var passengers = (await multi.ReadAsync<dynamic>()).ToList();
                   summary.Passengers = passengers.Count;

                   // Pricing
                   var pricing = await multi.ReadFirstOrDefaultAsync<BookingPricing>();

                   // Addons
                   var addons = (await multi.ReadAsync<dynamic>()).ToList();

                   return summary;
               }
               catch(Exception ex)
               {
                   throw;
               }
           }*/

        public async Task<bool> ChangeFlightDateAsync(int bookingId, int flightId, DateTime newDate, int changedBy, string reason)
        {
            try
            {
                using var connection = CreateConnection();
                var result = await connection.QueryFirstOrDefaultAsync<dynamic>(
                    "sp_ChangeFlightDate",
                    new
                    {
                        BookingId = bookingId,
                        FlightId = flightId,
                        NewDepartureDate = newDate,
                        ChangedBy = changedBy,
                        Reason = reason
                    },
                    commandType: CommandType.StoredProcedure
                );

                return result != null;
            }
            catch(Exception ex)
            {
                throw;
            }
        }

        public async Task<bool> UpdatePassengerDetailsAsync(int passengerId, string email, string phone,
            string passport, DateTime? expiry)
        {
            using var connection = CreateConnection();
            var result = await connection.ExecuteScalarAsync<int>(
                "sp_UpdatePassengerDetails",
                new
                {
                    PassengerId = passengerId,
                    Email = email,
                    Phone = phone,
                    PassportNumber = passport,
                    PassportExpiry = expiry
                },
                commandType: CommandType.StoredProcedure
            );

            return result == 1;
        }

        public async Task<bool> ChangeSeatAsync(int bookingPassengerId, int flightId, string newSeat, int changedBy)
        {
            using var connection = CreateConnection();
            var result = await connection.QueryFirstOrDefaultAsync<dynamic>(
                "sp_ChangeSeat",
                new
                {
                    BookingPassengerId = bookingPassengerId,
                    FlightId = flightId,
                    NewSeatNumber = newSeat,
                    ChangedBy = changedBy
                },
                commandType: CommandType.StoredProcedure
            );

            return result != null && result.Success == 1;
        }

        public async Task<decimal> CalculateCancellationFeeAsync(int bookingId)
        {
            using var connection = CreateConnection();
            var result = await connection.QueryFirstOrDefaultAsync<dynamic>(
                "sp_CalculateCancellationFee",
                new { BookingId = bookingId },
                commandType: CommandType.StoredProcedure
            );

            return result?.CancellationFee ?? 0;
        }

        // ==================== REFUNDS ====================

        public async Task<(int RefundId, string RefundReference)> CreateRefundRequestAsync(
            int bookingId, int paymentId, decimal refundAmount, decimal cancellationFee,
            string reason, string bankAccount, string bankName, int requestedBy)
        {
            using var connection = CreateConnection();
            var result = await connection.QueryFirstOrDefaultAsync<dynamic>(
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

        public async Task<bool> ProcessRefundAsync(int refundId, string stripeRefundId, string status)
        {
            using var connection = CreateConnection();
            await connection.ExecuteAsync(
                "sp_ProcessRefund",
                new
                {
                    RefundId = refundId,
                    StripeRefundId = stripeRefundId,
                    Status = status
                },
                commandType: CommandType.StoredProcedure
            );

            return true;
        }


        //public async Task<Booking> CreateBookingAsync(Booking booking, )
        //{
        //    using var connection = CreateConnection();

        //    var sql = @"
        //        INSERT INTO Bookings (
        //            BookingReference, PNR, UserId, TotalAmount, Currency,
        //            BookingEmail, BookingPhone, SpecialRequests, BookingStatus, PaymentStatus
        //        )
        //        VALUES (
        //            @BookingReference, @PNR, @UserId, @TotalAmount, @Currency,
        //            @BookingEmail, @BookingPhone, @SpecialRequests, @BookingStatus, @PaymentStatus
        //        );
        //        SELECT CAST(SCOPE_IDENTITY() as int);";

        //    var bookingId = await connection.ExecuteScalarAsync<int>(sql, booking);
        //    booking.BookingId = bookingId;

        //    return booking;

        //}

        public async Task<Booking> CreateBookingAsync(
            Booking booking,
            IDbConnection connection,
            IDbTransaction transaction,
            CancellationToken cancellationToken = default)
        {
            const string sql = @"
        INSERT INTO Bookings (
            BookingReference, PNR, UserId, TotalAmount, Currency,
            BookingEmail, BookingPhone, SpecialRequests, BookingStatus,
            PaymentStatus, BookingDate, CreatedAt
        )
        VALUES (
            @BookingReference, @PNR, @UserId, @TotalAmount, @Currency,
            @BookingEmail, @BookingPhone, @SpecialRequests, @BookingStatus,
            @PaymentStatus, @BookingDate, @CreatedAt
        );
        SELECT CAST(SCOPE_IDENTITY() AS INT);";

            var bookingId = await connection.ExecuteScalarAsync<int>(
                new CommandDefinition(sql, booking, transaction,
                    cancellationToken: cancellationToken));

            booking.BookingId = bookingId;
            return booking;
        }

        public async Task<Booking> GetBookingByIdAsync(int bookingId)
        {
            using var connection = CreateConnection();

            var sql = @"
                SELECT 
                    BookingId, BookingReference, PNR, UserId, TotalAmount, Currency,
                    BookingEmail, BookingPhone, SpecialRequests, BookingStatus, PaymentStatus,
                    PaymentIntentId, BookingDate, CreatedAt, UpdatedAt
                FROM Bookings
                WHERE BookingId = @BookingId";

            var booking = await connection.QueryFirstOrDefaultAsync<Booking>(sql, new { BookingId = bookingId });

            return booking;
        }

        public async Task<Booking> GetBookingByReferenceAsync(string bookingReference)
        {
            using var connection = CreateConnection();

            var sql = @"
                SELECT 
                    BookingId, BookingReference, PNR, UserId, TotalAmount, Currency,
                    BookingEmail, BookingPhone, SpecialRequests, BookingStatus, PaymentStatus,
                    PaymentIntentId, BookingDate, CreatedAt, UpdatedAt
                FROM Bookings
                WHERE BookingReference = @BookingReference";

            var booking = await connection.QueryFirstOrDefaultAsync<Booking>(sql, new { BookingReference = bookingReference });

            return booking;
        }
        public async Task<(string BookingReference, string PNR)> GenerateBookingIdentifiersAsync(CancellationToken cancellationToken = default)
        {
            using var connection = CreateConnection();
            //using var multi = await connection.QueryMultipleAsync(
            //    "sp_GenerateBookingReference",
            //    commandType: CommandType.StoredProcedure
            //);
            using var multi = await connection.QueryMultipleAsync(
            new CommandDefinition(
                 "sp_GenerateBookingReference",
                 commandType: CommandType.StoredProcedure,
                 cancellationToken: cancellationToken));

            var bookingRef = await multi.ReadFirstOrDefaultAsync<string>();

            var pnr = await multi.ReadFirstOrDefaultAsync<string>();

            return (bookingRef, pnr);
        }

        public async Task<Booking> GetBookingByPNRAsync(string pnr)
        {
            using var connection = CreateConnection();

            var sql = @"
                SELECT 
                    BookingId, BookingReference, PNR, UserId, TotalAmount, Currency,
                    BookingEmail, BookingPhone, SpecialRequests, BookingStatus, PaymentStatus,
                    PaymentIntentId, BookingDate, CreatedAt, UpdatedAt
                FROM Bookings
                WHERE PNR = @PNR";

            var booking = await connection.QueryFirstOrDefaultAsync<Booking>(sql, new { PNR = pnr });

            return booking;
        }
        public async Task<(IEnumerable<Booking> Bookings, int TotalRecords)> GetUserBookingsAsync(
            int userId, int pageNumber, int pageSize)
        {
            using var connection = CreateConnection();

            var result = await connection.QueryMultipleAsync(
                "sp_GetUserBookings",
                new { UserId = userId, PageNumber = pageNumber, PageSize = pageSize },
                commandType: CommandType.StoredProcedure
            );

            var bookings = await result.ReadAsync<Booking>();
            var totalRecords = await result.ReadFirstOrDefaultAsync<int>();

            return (bookings, totalRecords);
        }

        public async Task<Booking> ConfirmBookingAsync(int bookingId, string paymentIntentId)
        {
            using var connection = CreateConnection();

            await connection.ExecuteAsync(
                "sp_ConfirmBooking",
                new { BookingId = bookingId, PaymentIntentId = paymentIntentId },
                commandType: CommandType.StoredProcedure
            );

            return await GetBookingByIdAsync(bookingId);
        }

        public async Task<bool> CancelBookingAsync(int bookingId, int cancelledBy, decimal refundAmount)
        {
            using var connection = CreateConnection();

            var sql = @"
                UPDATE Bookings
                SET BookingStatus = 'Cancelled',
                    UpdatedAt = GETDATE()
                WHERE BookingId = @BookingId;

                INSERT INTO BookingHistory (BookingId, Action, OldValues, NewValues, ChangedBy)
                VALUES (@BookingId, 'Cancelled', 
                    (SELECT BookingStatus FROM Bookings WHERE BookingId = @BookingId), 
                    'Cancelled', @CancelledBy);";

            var rowsAffected = await connection.ExecuteAsync(sql, new
            {
                BookingId = bookingId,
                CancelledBy = cancelledBy
            });

            return rowsAffected > 0;
        }

        public async Task<bool> UpdateBookingAsync(Booking booking)
        {
            using var connection = CreateConnection();

            var sql = @"
                UPDATE Bookings
                SET TotalAmount = @TotalAmount,
                    BookingEmail = @BookingEmail,
                    BookingPhone = @BookingPhone,
                    BookingStatus = @BookingStatus,
                    PaymentStatus = @PaymentStatus,
                    PaymentIntentId = @PaymentIntentId,
                    UpdatedAt = GETDATE()
                WHERE BookingId = @BookingId";

            var rowsAffected = await connection.ExecuteAsync(sql, booking);

            return rowsAffected > 0;
        }

        public async Task<bool> AddFlightToBookingAsync(int bookingId, int flightId, int flightSegment, IDbConnection connection,
            IDbTransaction transaction,
            CancellationToken cancellationToken = default)
        {
            //using var connection = CreateConnection();

            //var sql = @"
            //    INSERT INTO BookingFlights (BookingId, FlightId, FlightSegment)
            //    VALUES (@BookingId, @FlightId, @FlightSegment)";

            //var rowsAffected = await connection.ExecuteAsync(sql, new
            //{
            //    BookingId = bookingId,
            //    FlightId = flightId,
            //    FlightSegment = flightSegment
            //});
            const string sql = @"
                    INSERT INTO BookingFlights (BookingId, FlightId, FlightSegment)
                    VALUES (@BookingId, @FlightId, @FlightSegment)";

            var rows = await connection.ExecuteAsync(
                new CommandDefinition(sql,
                    new { BookingId = bookingId, FlightId = flightId, FlightSegment = flightSegment },
                    transaction,
                    cancellationToken: cancellationToken));

            return rows > 0;
        }

        public async Task<IEnumerable<BookingFlight>> GetBookingFlightsAsync(int bookingId, CancellationToken cancellationToken = default)
        {
            using var connection = CreateConnection();

            //var sql = @"
            //    SELECT 
            //        bf.BookingFlightId,
            //        bf.BookingId,
            //        bf.FlightId,
            //        bf.FlightSegment,
            //        f.FlightNumber,
            //        f.DepartureDateTime,
            //        f.ArrivalDateTime,
            //        oa.AirportCode AS OriginCode,
            //        da.AirportCode AS DestinationCode
            //    FROM BookingFlights bf
            //    INNER JOIN Flights f ON bf.FlightId = f.FlightId
            //    INNER JOIN Routes r ON f.RouteId = r.RouteId
            //    INNER JOIN Airports oa ON r.OriginAirportId = oa.AirportId
            //    INNER JOIN Airports da ON r.DestinationAirportId = da.AirportId
            //    WHERE bf.BookingId = @BookingId
            //    ORDER BY bf.FlightSegment";

            //var flights = await connection.QueryAsync<BookingFlight>(sql, new { BookingId = bookingId });

            //return flights;

            const string sql = @"
                SELECT f.FlightId, f.FlightNumber, f.Origin, f.Destination,
                       f.DepartureTime, f.ArrivalTime, bf.FlightSegment
                FROM   BookingFlights bf
                INNER JOIN Flights f ON f.FlightId = bf.FlightId
                WHERE  bf.BookingId = @BookingId
                ORDER  BY bf.FlightSegment";

            return await connection.QueryAsync<BookingFlight>(
                new CommandDefinition(sql, new { BookingId = bookingId },
                    cancellationToken: cancellationToken));

        }
        public async Task<BookingPricing> CreatePricingAsync(BookingPricing pricing, IDbConnection connection,
            IDbTransaction transaction,
            CancellationToken cancellationToken = default)
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

            var pricingId = await connection.ExecuteScalarAsync<int>(
                new CommandDefinition(sql, pricing, transaction,
                    cancellationToken: cancellationToken));

            pricing.PricingId = pricingId;
            return pricing;
        }

    }

}
