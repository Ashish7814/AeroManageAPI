using AeroManage.BookingManagement.Application.DTOs;
using AeroManage.BookingManagement.Application.Queries.Tickets;
using AeroManage.BookingManagement.Infrastructure.Repositories.Interfaces;
using AeroManage.Shared.Service.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AeroManage.BookingManagement.Application.Handlers.Tickets
{
    public class GetPNRDetailsQueryHandler : IRequestHandler<GetPNRDetailsQuery, ApiResponse<PNRDetailsDto>>
    {
        private readonly IBookingRepository _repo;
        private readonly IPassengerRepository _passengerRepo;
        private readonly ICacheService _cache;
        private readonly ILogger<GetPNRDetailsQueryHandler> _logger;

        public GetPNRDetailsQueryHandler(
            IBookingRepository repo,
            IPassengerRepository passengerRepo,
            ICacheService cache,
            ILogger<GetPNRDetailsQueryHandler> logger)
        {
            _repo = repo;
            _passengerRepo = passengerRepo;
            _cache = cache;
            _logger = logger;
        }

        public async Task<ApiResponse<PNRDetailsDto>> Handle(GetPNRDetailsQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // Try cache first
                var cacheKey = $"pnr:{request.bookingId}";
                var cached = await _cache.GetAsync<PNRDetailsDto>(cacheKey);

                if (cached != null)
                {
                    return ApiResponse<PNRDetailsDto>.SuccessResponse(cached);
                }

                // Get booking
                var booking = await _repo.GetBookingByIdAsync(request.bookingId);
                if (booking == null)
                {
                    return ApiResponse<PNRDetailsDto>.ErrorResponse("Booking not found");
                }

                // Get passengers
                var passengers = await _passengerRepo.GetBookingPassengersAsync(request.bookingId, cancellationToken);

                // Get flights
                var flights = await _repo.GetBookingFlightsAsync(request.bookingId);

                // Build PNR details
                var pnrDetails = new PNRDetailsDto
                {
                    PNR = booking.PNR,
                    BookingReference = booking.BookingReference,
                    Booking = new BookingDto
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
                        ContactPhone = booking.BookingPhone
                    },
                    Passengers = passengers.Select(p => new PassengerTicketDto
                    {
                        BookingPassengerId = p.BookingPassengerId,
                        TicketNumber = p.TicketNumber,
                        Passenger = new PassengerDto
                        {
                            PassengerId = p.PassengerId,
                            FirstName = p.Passenger.FirstName,
                            LastName = p.Passenger.LastName,
                            Email = p.Passenger.Email,
                            Phone = p.Passenger.Phone
                        },
                        SeatNumber = p.SeatNumber,
                        SeatClass = p.SeatClass,
                        QRCode = p.QRCodePath
                    }).ToList(),
                    Flights = new List<FlightDetailsDto>() // Would be populated from flight repository
                };

                // Cache for 1 hour
                await _cache.SetAsync(cacheKey, pnrDetails, TimeSpan.FromHours(1));

                return ApiResponse<PNRDetailsDto>.SuccessResponse(pnrDetails, "PNR details retrieved successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting PNR details for booking {request.bookingId}");
                return ApiResponse<PNRDetailsDto>.ErrorResponse("Failed to retrieve PNR details", new[] { ex.Message });
            }
        }
    }
}
