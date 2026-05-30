using AeroManage.BookingManagement.Application.Commands.BoardingPass;
using AeroManage.BookingManagement.Application.DTOs;
using AeroManage.BookingManagement.Application.Hubs;
using AeroManage.BookingManagement.Infrastructure.Repositories.Interfaces;
using MediatR;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AeroManage.BookingManagement.Application.Handlers.BoardingPass
{
    public class UpdateBoardingPassGateCommandHandler : IRequestHandler<UpdateBoardingPassGateCommand, ApiResponse<int>>
    {
        private readonly IBoardingPassRepository _boardingPassRepo;
        private readonly IHubContext<BookingHub> _hubContext;
        private readonly ILogger<UpdateBoardingPassGateCommandHandler> _logger;

        public UpdateBoardingPassGateCommandHandler(
            IBoardingPassRepository boardingPassRepo,
            IHubContext<BookingHub> hubContext,
            ILogger<UpdateBoardingPassGateCommandHandler> logger)
        {
            _boardingPassRepo = boardingPassRepo;
            _hubContext = hubContext;
            _logger = logger;
        }

        public async Task<ApiResponse<int>> Handle(UpdateBoardingPassGateCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var passengersUpdated = await _boardingPassRepo.UpdateBoardingPassGateAsync(
                    request.dto.FlightId,
                    request.dto.NewGate,
                    request.dto.NewBoardingTime
                );

                // Broadcast gate change to all passengers on this flight
                await _hubContext.Clients.Group($"Flight_{request.dto.FlightId}")
                    .SendAsync("GateChanged", new
                    {
                        FlightId = request.dto.FlightId,
                        NewGate = request.dto.NewGate,
                        NewBoardingTime = request.dto.NewBoardingTime,
                        Message = $"Gate changed to {request.dto.NewGate}"
                    }, cancellationToken);

                return ApiResponse<int>.SuccessResponse(
                    passengersUpdated,
                    $"Updated {passengersUpdated} boarding passes"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating boarding pass gates");
                return ApiResponse<int>.ErrorResponse("Failed to update gates", new[] { ex.Message });
            }
        }
    }
}
