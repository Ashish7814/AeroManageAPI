using AeroManage.BookingManagement.Application.Commands.Seats;
using AeroManage.BookingManagement.Application.DTOs;
using AeroManage.BookingManagement.Infrastructure.Repositories.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AeroManage.BookingManagement.Application.Handlers.Seats
{
    public class ReleaseExpiredReservationsCommandHandler: IRequestHandler<ReleaseExpiredReservationsCommand, ApiResponse<int>>
    {
        private readonly ISeatRepository _seatRepo;
        private readonly ILogger<ReleaseExpiredReservationsCommandHandler> _logger;

        public ReleaseExpiredReservationsCommandHandler(
            ISeatRepository seatRepo,
            ILogger<ReleaseExpiredReservationsCommandHandler> logger)
        {
            _seatRepo = seatRepo;
            _logger = logger;
        }

        public async Task<ApiResponse<int>> Handle(ReleaseExpiredReservationsCommand request, CancellationToken cancellationToken)
        {
            try
            {
                await _seatRepo.ReleaseExpiredReservationsAsync();

                _logger.LogInformation("Expired seat reservations released");

                return ApiResponse<int>.SuccessResponse(0, "Expired reservations released");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error releasing expired reservations");
                return ApiResponse<int>.ErrorResponse(
                    "Failed to release expired reservations", new[] { ex.Message });
            }
        }
    }
}
