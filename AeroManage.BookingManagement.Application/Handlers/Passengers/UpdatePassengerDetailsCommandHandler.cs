using AeroManage.BookingManagement.Application.Commands.Passengers;
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

namespace AeroManage.BookingManagement.Application.Handlers.Passengers
{
    public class UpdatePassengerDetailsCommandHandler : IRequestHandler<UpdatePassengerDetailsCommand, ApiResponse<PassengerDto>>
    {
        private readonly IBookingRepository _repo;
        private readonly IPassengerRepository _passengerRepo;
        private readonly ICacheService _cache;
        private readonly IHubContext<BookingHub> _hubContext;
       /* private readonly IMessageQueueService _messageQueue;*/
        private readonly ILogger<UpdatePassengerDetailsCommandHandler> _logger;

        public UpdatePassengerDetailsCommandHandler(
            IBookingRepository repo,
            IPassengerRepository passengerRepo,
            ICacheService cache,
            IHubContext<BookingHub> hubContext,
            /*IMessageQueueService messageQueue,*/
            ILogger<UpdatePassengerDetailsCommandHandler> logger)
        {
            _repo = repo;
            _passengerRepo = passengerRepo;
            _cache = cache;
            _hubContext = hubContext;
         /*   _messageQueue = messageQueue;*/
            _logger = logger;
        }

        public async Task<ApiResponse<PassengerDto>> Handle(UpdatePassengerDetailsCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Update passenger details
                var result = await _repo.UpdatePassengerDetailsAsync(
                    request.dto.PassengerId,
                    request.dto.Email,
                    request.dto.Phone,
                    request.dto.PassportNumber,
                    request.dto.PassportExpiry
                );

                if (!result)
                {
                    return ApiResponse<PassengerDto>.ErrorResponse("Failed to update passenger details");
                }

                // Get updated passenger
                var passenger = await _passengerRepo.GetPassengerByIdAsync(request.dto.PassengerId, request.dto.Email);

                // Invalidate cache
                await _cache.RemoveAsync($"booking:{request.bookingId}");
                await _cache.RemoveAsync($"booking:summary:{request.bookingId}");

                // Publish modification notification
               /* await _messageQueue.PublishAsync("booking.modified", new
                {
                    BookingId = request.bookingId,
                    ModificationType = "PassengerUpdate",
                    PassengerId = request.dto.PassengerId
                });*/

                // SignalR notification
                await _hubContext.Clients.Group($"Booking_{request.bookingId}")
                    .SendAsync("BookingModified", new
                    {
                        BookingId = request.bookingId,
                        ModificationType = "PassengerUpdate",
                        Message = "Passenger details updated successfully"
                    }, cancellationToken);

                var dto = new PassengerDto
                {
                    PassengerId = passenger.PassengerId,
                    FirstName = passenger.FirstName,
                    LastName = passenger.LastName,
                    Email = passenger.Email,
                    Phone = passenger.Phone,
                    PassportNumber = passenger.PassportNumber,
                    PassportExpiry = passenger.PassportExpiry
                };

                return ApiResponse<PassengerDto>.SuccessResponse(dto, "Passenger details updated successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating passenger {request.dto.PassengerId}");
                return ApiResponse<PassengerDto>.ErrorResponse("Failed to update passenger details", new[] { ex.Message });
            }
        }
    }
}
