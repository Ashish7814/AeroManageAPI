using AeroManage.BookingManagement.Application.DTOs;
using AeroManage.BookingManagement.Application.Queries.Tickets;
using AeroManage.BookingManagement.Application.Services.Interfaces;
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
    public class GetPrintableTicketQueryHandler : IRequestHandler<GetPrintableTicketQuery, ApiResponse<PrintableTicketDto>>
    {
        private readonly IBookingRepository _bookingRepo;
        private readonly IPassengerRepository _passengerRepo;
        private readonly IBookingRepository _extendedRepo;
        private readonly IQRCodeService _qrService;
        private readonly ICacheService _cache;
        private readonly ILogger<GetPrintableTicketQueryHandler> _logger;

        public GetPrintableTicketQueryHandler(
            IBookingRepository bookingRepo,
            IPassengerRepository passengerRepo,
            IBookingRepository extendedRepo,
            IQRCodeService qrService,
            ICacheService cache,
            ILogger<GetPrintableTicketQueryHandler> logger)
        {
            _bookingRepo = bookingRepo;
            _passengerRepo = passengerRepo;
            _extendedRepo = extendedRepo;
            _qrService = qrService;
            _cache = cache;
            _logger = logger;
        }

        public async Task<ApiResponse<PrintableTicketDto>> Handle(GetPrintableTicketQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // Try cache first
                var cacheKey = $"printable:ticket:{request.bookingId}";
                var cached = await _cache.GetAsync<PrintableTicketDto>(cacheKey);

                if (cached != null)
                {
                    return ApiResponse<PrintableTicketDto>.SuccessResponse(cached);
                }

                // Get booking
                var booking = await _bookingRepo.GetBookingByIdAsync(request.bookingId);
                if (booking == null)
                {
                    return ApiResponse<PrintableTicketDto>.ErrorResponse("Booking not found");
                }

                // Get passengers
                var passengers = await _passengerRepo.GetBookingPassengersAsync(request.bookingId, cancellationToken);

                // Get flights
                var flights = await _bookingRepo.GetBookingFlightsAsync(request.bookingId);

                // Generate QR code for booking
                var qrData = $"{booking.PNR}|{booking.BookingReference}|{flights.FirstOrDefault()?.FlightNumber}";
                var qrCode = await _qrService.GenerateQRCodeBase64Async(qrData);

                // Generate barcode (simple format)
                var barcode = $"*{booking.BookingReference}*";

                // Build printable ticket DTO
                var printableTicket = new PrintableTicketDto
                {
                    Booking = new BookingDto
                    {
                        BookingId = booking.BookingId,
                        BookingReference = booking.BookingReference,
                        PNR = booking.PNR,
                        BookingStatus = booking.BookingStatus,
                        TotalAmount = booking.TotalAmount,
                        Currency = booking.Currency,
                        BookingDate = booking.BookingDate,
                        ContactEmail = booking.BookingEmail,
                        ContactPhone = booking.BookingPhone
                    },
                    Passengers = passengers.Select(p => new PassengerTicketDto
                    {
                        BookingPassengerId = p.BookingPassengerId,
                        TicketNumber = p.TicketNumber ?? $"TKT{booking.BookingId:D6}{p.BookingPassengerId:D4}",
                        Passenger = new PassengerDto
                        {
                            PassengerId = p.PassengerId,
                            FirstName = p.Passenger.FirstName,
                            LastName = p.Passenger.LastName,
                            Email = p.Passenger.Email,
                            Phone = p.Passenger.Phone
                        },
                        SeatNumber = p.SeatNumber,
                        SeatClass = p.SeatClass
                    }).ToList(),
                    Flights = new List<FlightDetailsDto>(), // Would be populated from flight details
                    QRCode = qrCode,
                    Barcode = barcode
                };

                // Cache for 1 hour
                await _cache.SetAsync(cacheKey, printableTicket, TimeSpan.FromHours(1));

                return ApiResponse<PrintableTicketDto>.SuccessResponse(printableTicket, "Printable ticket generated successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error generating printable ticket for booking {request.bookingId}");
                return ApiResponse<PrintableTicketDto>.ErrorResponse("Failed to generate printable ticket", new[] { ex.Message });
            }
        }
    }
}
