using AeroManage.BookingManagement.Application.Commands.Date;
using AeroManage.BookingManagement.Application.DTOs;
using AeroManage.BookingManagement.Application.Hubs;
using AeroManage.BookingManagement.Infrastructure.Repositories.Interfaces;
using AeroManage.Shared.Service.Interfaces;
using MediatR;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AeroManage.BookingManagement.Application.Handlers.Date
{
    public class ChangeDateCommandHandler : IRequestHandler<ChangeDateCommand, ApiResponse<BookingDto>>
    {
        private readonly IBookingRepository _repo;
        private readonly ICacheService _cache;
        private readonly IHubContext<BookingHub> _hubContext;
   /*     private readonly IMessageQueueService _messageQueue;*/
        private readonly ILogger<ChangeDateCommandHandler> _logger;

        public ChangeDateCommandHandler(
            IBookingRepository repo,
            ICacheService cache,
            IHubContext<BookingHub> hubContext,
            /*IMessageQueueService messageQueue,*/
            ILogger<ChangeDateCommandHandler> logger)
        {
            _repo = repo;
            _cache = cache;
            _hubContext = hubContext;
            /*_messageQueue = messageQueue;*/
            _logger = logger;
        }

        public async Task<ApiResponse<BookingDto>> Handle(ChangeDateCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _repo.ChangeFlightDateAsync(
                    request.bookingId,
                    request .dto.FlightId,
                    request.dto.NewDepartureDate,
                    request.dto.ChangedBy,
                    request.dto.Reason
                );

                if (!result)
                {
                    return ApiResponse<BookingDto>.ErrorResponse("Failed to change flight date");
                }

                // Invalidate cache
                await _cache.RemoveAsync($"booking:{request.bookingId}");
                await _cache.RemoveAsync($"booking:summary:{request.bookingId}");

                // Publish modification email
               /* await _messageQueue.PublishAsync("booking.modified", new
                {
                    BookingId = request.bookingId,
                    ModificationType = "DateChange",
                    NewDate = request.dto.NewDepartureDate
                });*/

                // SignalR notification
                await _hubContext.Clients.Group($"Booking_{request.bookingId}")
                    .SendAsync("BookingModified", new
                    {
                        BookingId = request.bookingId,
                        ModificationType = "DateChange"
                    }, cancellationToken);

                return ApiResponse<BookingDto>.SuccessResponse(new BookingDto { BookingId = request.bookingId },
                    "Flight date changed successfully. Modification fee applied.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error changing flight date");
                return ApiResponse<BookingDto>.ErrorResponse("Failed to change flight date", new[] { ex.Message });
            }
        }
    }
}
