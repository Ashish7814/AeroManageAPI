using AeroManage.BookingManagement.Domain.Entities;
using AeroManage.BookingManagement.Domain.Interfaces;
using AeroManage.BookingManagement.Infrastructure.Repositories.Interfaces;
using Dapper;

namespace AeroManage.BookingManagement.Infrastructure.Repositories.Implementation
{
    public class FlightDetailsRepository : IFlightDetailsRepository
    {
        private readonly IDapperUnitOfWork _unitOfWork;

        public FlightDetailsRepository(IDapperUnitOfWork uow) => _unitOfWork = uow;

        public async Task<bool> AddFlightToBookingAsync(int bookingId, int flightId, int flightSegment, CancellationToken cancellationToken = default)
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

            var rows = await _unitOfWork.Connection.ExecuteAsync(
               new CommandDefinition(sql,
                   new { BookingId = bookingId, FlightId = flightId, FlightSegment = flightSegment },
                   _unitOfWork.Transaction,
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
