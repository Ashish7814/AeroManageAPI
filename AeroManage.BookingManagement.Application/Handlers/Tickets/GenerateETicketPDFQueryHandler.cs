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
    public class GenerateETicketPDFQueryHandler : IRequestHandler<GenerateETicketPDFQuery, ApiResponse<byte[]>>
    {
        private readonly IBookingRepository _bookingRepo;
        private readonly IPassengerRepository _passengerRepo;
        private readonly IPdfService _pdfService;
        private readonly IQRCodeService _qrService;
        private readonly ILogger<GenerateETicketPDFQueryHandler> _logger;

        public GenerateETicketPDFQueryHandler(
            IBookingRepository bookingRepo,
            IPassengerRepository passengerRepo,
            IPdfService pdfService,
            IQRCodeService qrService,
            ILogger<GenerateETicketPDFQueryHandler> logger)
        {
            _bookingRepo = bookingRepo;
            _passengerRepo = passengerRepo;
            _pdfService = pdfService;
            _qrService = qrService;
            _logger = logger;
        }

        public async Task<ApiResponse<byte[]>> Handle(GenerateETicketPDFQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // Get booking
                var booking = await _bookingRepo.GetBookingByIdAsync(request.bookingId);
                if (booking == null)
                {
                    return ApiResponse<byte[]>.ErrorResponse("Booking not found");
                }

                // Get passengers and flights
                var passengers = await _passengerRepo.GetBookingPassengersAsync(request.bookingId, cancellationToken);
                var flights = await _bookingRepo.GetBookingFlightsAsync(request.bookingId);

                if (!passengers.Any())
                {
                    return ApiResponse<byte[]>.ErrorResponse("No passengers found for this booking");
                }

                // Use MemoryStream to combine all PDFs
                using var combinedStream = new MemoryStream();

                foreach (var passenger in passengers)
                {
                    // Generate QR code
                    var qrData = $"{booking.PNR}|{passenger.Passenger.FirstName} {passenger.Passenger.LastName}|{flights.FirstOrDefault()?.FlightNumber}";
                    var qrCode = await _qrService.GenerateQRCodeBase64Async(qrData);

                    // Create e-ticket DTO
                    var eTicket = new ETicketDto
                    {
                        TicketNumber = passenger.TicketNumber ?? $"TKT{booking.BookingId:D6}{passenger.BookingPassengerId:D4}",
                        BookingReference = booking.BookingReference,
                        PNR = booking.PNR,
                        Passenger = new PassengerDto
                        {
                            PassengerId = passenger.PassengerId,
                            FirstName = passenger.Passenger.FirstName,
                            LastName = passenger.Passenger.LastName,
                            Email = passenger.Passenger.Email,
                            Phone = passenger.Passenger.Phone
                        },
                        Flight = new FlightDetailsDto
                        {
                            FlightNumber = flights.FirstOrDefault()?.FlightNumber,
                            DepartureDateTime = passenger.Booking.flight.DepartureDateTime,
                            ArrivalDateTime = passenger.Booking.flight.ArrivalDateTime,
                            Origin = new AirportInfoDto
                            {
                                AirportCode = passenger.Booking.flight.OriginCode
                            },
                            Destination = new AirportInfoDto
                            {
                                AirportCode = passenger.Booking.flight.DestinationCode
                            }
                        },
                        QRCode = qrCode
                    };

                    // Generate PDF for this passenger
                    var pdfPath = await _pdfService.GenerateETicketAsync(eTicket);
                    var pdfBytes = await File.ReadAllBytesAsync(pdfPath);
                    await combinedStream.WriteAsync(pdfBytes, 0, pdfBytes.Length);
                }

                var finalPdfBytes = combinedStream.ToArray();

                _logger.LogInformation($"Generated e-ticket PDF for booking {request.bookingId}");

                return ApiResponse<byte[]>.SuccessResponse(finalPdfBytes, "E-tickets generated successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error generating e-ticket PDF for booking {request.bookingId}");
                return ApiResponse<byte[]>.ErrorResponse("Failed to generate e-tickets", new[] { ex.Message });
            }
        }
    }
}
