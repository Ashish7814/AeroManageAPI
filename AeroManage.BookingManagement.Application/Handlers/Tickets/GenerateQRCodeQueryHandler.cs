using AeroManage.BookingManagement.Application.DTOs;
using AeroManage.BookingManagement.Application.Queries.Tickets;
using AeroManage.BookingManagement.Application.Services.Interfaces;
using AeroManage.BookingManagement.Infrastructure.Repositories.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AeroManage.BookingManagement.Application.Handlers.Tickets
{
    public class GenerateQRCodeQueryHandler : IRequestHandler<GenerateQRCodeQuery, ApiResponse<string>>
    {
        private readonly IBookingRepository _bookingRepo;
        private readonly IPassengerRepository _passengerRepo;
        private readonly IQRCodeService _qrService;
        private readonly ILogger<GenerateQRCodeQueryHandler> _logger;

        public GenerateQRCodeQueryHandler(
            IBookingRepository bookingRepo,
            IPassengerRepository passengerRepo,
            IQRCodeService qrService,
            ILogger<GenerateQRCodeQueryHandler> logger)
        {
            _bookingRepo = bookingRepo;
            _passengerRepo = passengerRepo;
            _qrService = qrService;
            _logger = logger;
        }

        public async Task<ApiResponse<string>> Handle(GenerateQRCodeQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // Get booking
                var booking = await _bookingRepo.GetBookingByIdAsync(request.bookingId);
                if (booking == null)
                {
                    return ApiResponse<string>.ErrorResponse("Booking not found");
                }

                // Get flights
                var flights = await _bookingRepo.GetBookingFlightsAsync(request.bookingId);
                var firstFlight = flights.FirstOrDefault();

                string qrData;
                string fileName;

                if (request.passengerId.HasValue)
                {
                    // Generate QR for specific passenger
                    var passengers = await _passengerRepo.GetBookingPassengersAsync(request.bookingId, cancellationToken);
                    var passenger = passengers.FirstOrDefault(p => p.BookingPassengerId == request.passengerId.Value);

                    if (passenger == null)
                    {
                        return ApiResponse<string>.ErrorResponse("Passenger not found");
                    }

                    qrData = $"{booking.PNR}|{passenger.Passenger.FirstName} {passenger.Passenger.LastName}|{firstFlight?.FlightNumber}|{passenger.SeatNumber}";
                    fileName = $"QR_{booking.BookingReference}_{passenger.BookingPassengerId}.png";
                }
                else
                {
                    // Generate QR for entire booking
                    qrData = $"{booking.PNR}|{booking.BookingReference}|{firstFlight?.FlightNumber}";
                    fileName = $"QR_{booking.BookingReference}.png";
                }

                // Generate QR code and get base64
                var qrCodeBase64 = await _qrService.GenerateQRCodeBase64Async(qrData);

                _logger.LogInformation($"Generated QR code for booking {request.bookingId}");

                return ApiResponse<string>.SuccessResponse(qrCodeBase64, "QR code generated successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error generating QR code for booking {request.bookingId}");
                return ApiResponse<string>.ErrorResponse("Failed to generate QR code", new[] { ex.Message });
            }
        }
    }
}
