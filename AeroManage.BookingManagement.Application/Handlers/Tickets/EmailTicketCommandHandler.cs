using AeroManage.BookingManagement.Application.DTOs;
using AeroManage.BookingManagement.Application.Queries.Tickets;
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
    public class EmailTicketCommandHandler : IRequestHandler<EmailTicketCommand, ApiResponse<bool>>
    {
        private readonly IBookingRepository _bookingRepo;
        private readonly IPassengerRepository _passengerRepo;
        private readonly IBookingRepository _extendedRepo;
        private readonly ITicketRepository _ticketRepository;
  /*      private readonly IMessageQueueService _messageQueue;*/
        private readonly ILogger<EmailTicketCommandHandler> _logger;

        public EmailTicketCommandHandler(
            IBookingRepository bookingRepo,
            IPassengerRepository passengerRepo,
            IBookingRepository extendedRepo,
            ITicketRepository ticketRepository,
            /*IMessageQueueService messageQueue,*/
            ILogger<EmailTicketCommandHandler> logger)
        {
            _bookingRepo = bookingRepo;
            _passengerRepo = passengerRepo;
            _extendedRepo = extendedRepo;
            _ticketRepository = ticketRepository;
          /*  _messageQueue = messageQueue;*/
            _logger = logger;
        }

        public async Task<ApiResponse<bool>> Handle(EmailTicketCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Get booking
                var booking = await _bookingRepo.GetBookingByIdAsync(request.bookingId);
                if (booking == null)
                {
                    return ApiResponse<bool>.ErrorResponse("Booking not found");
                }

                // Get passengers with e-tickets
                var passengers = await _passengerRepo.GetBookingPassengersAsync(request.bookingId, cancellationToken);
                var eTicketPaths = passengers
                    .Where(p => !string.IsNullOrEmpty(p.ETicketPath))
                    .Select(p => p.ETicketPath)
                    .ToList();

                if (!eTicketPaths.Any())
                {
                    return ApiResponse<bool>.ErrorResponse("No e-tickets available. Please generate tickets first.");
                }

                // Log email notification
                await _ticketRepository.LogEmailNotificationAsync(
                    request.bookingId,
                    "ETicket",
                    request.dto.Email,
                    $"Your E-Tickets - Booking {booking.BookingReference}",
                    "Pending"
                );

                // Publish to message queue for email sending
               /* await _messageQueue.PublishAsync("email.ticket", new
                {
                    BookingId = request.bookingId,
                    Email = request.dto.Email,
                    BookingReference = booking.BookingReference,
                    PNR = booking.PNR,
                    ETicketPaths = eTicketPaths,
                    IncludeInvoice = request.dto.IncludeInvoice,
                    IncludeBoardingPass = request.dto.IncludeBoardingPass
                });*/

                _logger.LogInformation($"E-ticket email queued for booking {request.bookingId} to {request.dto.Email}");

                return ApiResponse<bool>.SuccessResponse(true,
                    $"E-tickets will be sent to {request.dto.Email} shortly");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error emailing ticket for booking {request.bookingId}");
                return ApiResponse<bool>.ErrorResponse("Failed to send e-tickets", new[] { ex.Message });
            }
        }
    }

}
