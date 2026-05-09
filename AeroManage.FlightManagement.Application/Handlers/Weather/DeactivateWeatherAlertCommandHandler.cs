using AeroManage.FlightManagement.Application.Commands.Weather;
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

namespace AeroManage.FlightManagement.Application.Handlers.Weather
{
    public class DeactivateWeatherAlertCommandHandler : IRequestHandler<DeactivateWeatherAlertCommand, ApiResponse<bool>>
    {
        private readonly IWeatherAlertRepository _repository;
        private readonly IHubContext<FlightHub> _hubContext;

        public DeactivateWeatherAlertCommandHandler(
            IWeatherAlertRepository repository,
            IHubContext<FlightHub> hubContext)
        {
            _repository = repository;
            _hubContext = hubContext;
        }

        public async Task<ApiResponse<bool>> Handle(DeactivateWeatherAlertCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _repository.DeactivateAlertAsync(request.alertId);

                if (!result)
                {
                    return ApiResponse<bool>.ErrorResponse("Weather alert not found or already deactivated");
                }

                // Broadcast alert deactivation via SignalR
                await _hubContext.Clients.All.SendAsync("WeatherAlertDeactivated", new
                {
                    AlertId = request.alertId,
                    DeactivatedAt = DateTime.UtcNow
                }, cancellationToken);

                return ApiResponse<bool>.SuccessResponse(true, "Weather alert deactivated successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.ErrorResponse("Failed to deactivate weather alert", new[] { ex.Message });
            }
        }
    }
}
