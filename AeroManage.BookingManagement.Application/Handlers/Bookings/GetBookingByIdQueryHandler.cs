using AeroManage.BookingManagement.Application.DTOs;
using AeroManage.BookingManagement.Application.Queries.Bookings;
using AeroManage.BookingManagement.Infrastructure.Repositories.Interfaces;
using AeroManage.Shared.Service.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AeroManage.BookingManagement.Application.Handlers.Bookings
{
    public class GetBookingByIdQueryHandler : IRequestHandler<GetBookingByIdQuery, ApiResponse<BookingDto>>
    {
        private readonly IBookingRepository _bookingRepo;
        private readonly IPassengerRepository _passengerRepo;
        private readonly ICacheService _cache;
        private readonly ILogger<GetBookingByIdQueryHandler> _logger;

        public GetBookingByIdQueryHandler(
            IBookingRepository bookingRepo,
            IPassengerRepository passengerRepo,
            ICacheService cache,
            ILogger<GetBookingByIdQueryHandler> logger)
        {
            _bookingRepo = bookingRepo;
            _passengerRepo = passengerRepo;
            _cache = cache;
            _logger = logger;
        }

        public async Task<ApiResponse<BookingDto>> Handle(GetBookingByIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // Try cache first
                var cacheKey = $"booking:{request.bookingId}";
                var cached = await _cache.GetAsync<BookingDto>(cacheKey);

                if (cached != null)
                {
                    return ApiResponse<BookingDto>.SuccessResponse(cached);
                }

                // Get booking using repository
                var booking = await _bookingRepo.GetBookingByIdAsync(request.bookingId);

                if (booking == null)
                {
                    return ApiResponse<BookingDto>.ErrorResponse("Booking not found");
                }

                // Get flights using repository
                var flights = await _bookingRepo.GetBookingFlightsAsync(request.bookingId);

                // Get passengers using repository
                var passengers = await _passengerRepo.GetBookingPassengersAsync(request.bookingId, cancellationToken);

                var dto = new BookingDto
                {
                    BookingId = booking.BookingId,
                    BookingReference = booking.BookingReference,
                    PNR = booking.PNR,
                    BookingStatus = booking.BookingStatus,
                    TotalAmount = booking.TotalAmount,
                    Currency = booking.Currency,
                    PaymentStatus = booking.PaymentStatus,
                    BookingDate = booking.BookingDate,
                    ContactEmail = booking.BookingEmail,
                    ContactPhone = booking.BookingPhone,
                    PassengerCount = passengers.Count(),
                    FlightCount = flights.Count(),
                    Flights = flights.Select(f => new BookingFlightDto
                    {
                        FlightId = f.FlightId,
                        FlightNumber = f.FlightNumber,
                        FlightSegment = f.FlightSegment,
                        DepartureDateTime = f.DepartureDateTime,
                        ArrivalDateTime = f.ArrivalDateTime,
                        OriginCode = f.OriginCode,
                        DestinationCode = f.DestinationCode
                    }).ToList(),
                    Passengers = passengers.Select(p => new BookingPassengerDto
                    {
                        BookingPassengerId = p.BookingPassengerId,
                        FirstName = p.Passenger.FirstName,
                        LastName = p.Passenger.LastName,
                        PassengerType = p.PassengerType,
                        SeatClass = p.SeatClass,
                        SeatNumber = p.SeatNumber,
                        TicketNumber = p.TicketNumber,
                        MealPreference = p.MealPreference
                    }).ToList()
                };

                // Cache for 1 hour
                await _cache.SetAsync(cacheKey, dto, TimeSpan.FromHours(1));

                return ApiResponse<BookingDto>.SuccessResponse(dto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting booking {request.bookingId}");
                return ApiResponse<BookingDto>.ErrorResponse("Failed to retrieve booking", new[] { ex.Message });
            }
        }
    }
}
