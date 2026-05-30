using AeroManage.BookingManagement.Application.Commands.Meals;
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

namespace AeroManage.BookingManagement.Application.Handlers.Meals
{
    public class AddMealPreferenceCommandHandler : IRequestHandler<AddMealPreferenceCommand, ApiResponse<bool>>
    {
        private readonly IBookingRepository _repo;
        private readonly ICacheService _cache;
        private readonly ILogger<AddMealPreferenceCommandHandler> _logger;

        public AddMealPreferenceCommandHandler(
            IBookingRepository repo,
            ICacheService cache,
            ILogger<AddMealPreferenceCommandHandler> logger)
        {
            _repo = repo;
            _cache = cache;
            _logger = logger;
        }

        public async Task<ApiResponse<bool>> Handle(AddMealPreferenceCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _repo.AddMealPreferenceAsync(
                    request.dto.BookingPassengerId,
                    request.dto.MealType,
                    request.dto.SpecialInstructions
                );

                await _cache.RemoveAsync($"booking:summary:{request.bookingId}");

                return ApiResponse<bool>.SuccessResponse(result, "Meal preference added successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding meal preference");
                return ApiResponse<bool>.ErrorResponse("Failed to add meal preference", new[] { ex.Message });
            }
        }
    }
}
