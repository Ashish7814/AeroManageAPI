using AeroManage.BookingManagement.Domain.Entities;
using AeroManage.BookingManagement.Domain.Interfaces;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AeroManage.BookingManagement.Infrastructure.Repositories.Implementation
{
    public class FlightDetailsRepository : IFlightDetailsRepository
    {
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

        public Task<bool> ChangeFlightDateAsync(int bookingId, int flightId, DateTime newDate, int changedBy, string reason)
        {
            throw new NotImplementedException();
        }

        public Task<FlightDetails> GetFlightDetailsAsync(int flightId)
        {
            throw new NotImplementedException();
        }

        public Task<SeatAvailability> GetSeatAvailabilityAsync(int flightId, string seatClass)
        {
            throw new NotImplementedException();
        }
    }
}
