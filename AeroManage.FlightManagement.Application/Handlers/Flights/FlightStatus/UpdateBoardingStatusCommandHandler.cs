using AeroManage.FlightManagement.Application.Commands.Flights.FlightStatus;
using AeroManage.FlightManagement.Application.DTOs;
using AeroManage.FlightManagement.Application.Hubs;
using AeroManage.FlightManagement.Infrastructure.Repositories.Interfaces;
using MediatR;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AeroManage.FlightManagement.Application.Handlers.Flights.FlightStatus
{
    public class UpdateBoardingStatusCommandHandler : IRequestHandler<UpdateBoardingStatusCommand, ApiResponse<FlightDto>>
    {
        private readonly IFlightDashboardRepository _repository;
        private readonly IHubContext<FlightHub> _hubContext;

        public UpdateBoardingStatusCommandHandler(
            IFlightDashboardRepository repository,
            IHubContext<FlightHub> hubContext)
        {
            _repository = repository;
            _hubContext = hubContext;
        }

        public async Task<ApiResponse<FlightDto>> Handle(UpdateBoardingStatusCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var flight = await _repository.UpdateBoardingStatusAsync(
                    request.dto.FlightId,
                    request.dto.BoardingStatus,
                    request.dto.UpdatedBy
                );

                var dto = new FlightDto
                {
                    FlightId = flight.FlightId,
                    FlightNumber = flight.FlightNumber,
                    FlightStatus = flight.FlightStatus
                };

                // Broadcast boarding status via SignalR
                await _hubContext.Clients.Group($"Flight_{request.dto.FlightId}")
                    .SendAsync("BoardingStatusUpdated", new
                    {
                        request.dto.FlightId,
                        flight.FlightNumber,
                        request.dto.BoardingStatus
                    }, cancellationToken);

                return ApiResponse<FlightDto>.SuccessResponse(dto, "Boarding status updated");
            }
            catch (Exception ex)
            {
                return ApiResponse<FlightDto>.ErrorResponse("Failed to update boarding status", new[] { ex.Message });
            }
        }
    }
}
