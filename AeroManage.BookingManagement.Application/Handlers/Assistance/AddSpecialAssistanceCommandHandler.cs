using AeroManage.BookingManagement.Application.Commands.Assistance;
using AeroManage.BookingManagement.Application.DTOs;
using AeroManage.BookingManagement.Infrastructure.Repositories.Interfaces;
using AeroManage.Shared.Service.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AeroManage.BookingManagement.Application.Handlers.Assistance
{
    public class AddSpecialAssistanceCommandHandler : IRequestHandler<AddSpecialAssistanceCommand, ApiResponse<bool>>
    {
        private readonly IBookingRepository _repo;
        private readonly ICacheService _cache;
        private readonly ILogger<AddSpecialAssistanceCommandHandler> _logger;

        public AddSpecialAssistanceCommandHandler(
            IBookingRepository repo,
            ICacheService cache,
            ILogger<AddSpecialAssistanceCommandHandler> logger)
        {
            _repo = repo;
            _cache = cache;
            _logger = logger;
        }

        public async Task<ApiResponse<bool>> Handle(AddSpecialAssistanceCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _repo.AddSpecialAssistanceAsync(
                    request.dto.PassengerId,
                    request.dto.AssistanceType,
                    request.dto.Details
                );

                await _cache.RemoveAsync($"booking:summary:{request.bookingId}");

                return ApiResponse<bool>.SuccessResponse(result, "Special assistance added successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding special assistance");
                return ApiResponse<bool>.ErrorResponse("Failed to add special assistance", new[] { ex.Message });
            }
        }
    }
}
