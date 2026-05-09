using AeroManage.BookingManagement.Application.DTOs;
using AeroManage.BookingManagement.Application.Mappers;
using AeroManage.BookingManagement.Application.Queries.Seats;
using AeroManage.BookingManagement.Infrastructure.Repositories.Interfaces;
using AeroManage.Shared.Service.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AeroManage.BookingManagement.Application.Handlers.Seats
{
    public class GetSeatByIdQueryHandler
        : IRequestHandler<GetSeatByIdQuery, ApiResponse<SeatDto>>
    {
        private readonly ISeatRepository _seatRepo;
        private readonly ICacheService _cache;
        private readonly ILogger<GetSeatByIdQueryHandler> _logger;

        public GetSeatByIdQueryHandler(
            ISeatRepository seatRepo,
            ICacheService cache,
            ILogger<GetSeatByIdQueryHandler> logger)
        {
            _seatRepo = seatRepo;
            _cache = cache;
            _logger = logger;
        }

        public async Task<ApiResponse<SeatDto>> Handle(
            GetSeatByIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var cacheKey = $"seat:id:{request.seatId}";
                var cached = await _cache.GetAsync<SeatDto>(cacheKey);
                if (cached != null)
                    return ApiResponse<SeatDto>.SuccessResponse(cached);

                var seat = await _seatRepo.GetSeatByIdAsync(request.seatId);
                if (seat == null)
                    return ApiResponse<SeatDto>.ErrorResponse("Seat not found");

                var dto = Mapper.MapToDto(seat);
                await _cache.SetAsync(cacheKey, dto, TimeSpan.FromHours(24));

                return ApiResponse<SeatDto>.SuccessResponse(dto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting seat {request.seatId}");
                return ApiResponse<SeatDto>.ErrorResponse("Failed to retrieve seat", new[] { ex.Message });
            }
        }
    }
}
