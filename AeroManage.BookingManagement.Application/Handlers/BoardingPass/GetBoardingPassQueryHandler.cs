using AeroManage.BookingManagement.Application.DTOs;
using AeroManage.BookingManagement.Application.Queries.BoardingPass;
using AeroManage.BookingManagement.Domain.Entities;
using AeroManage.BookingManagement.Infrastructure.Repositories.Interfaces;
using AeroManage.Shared.Service.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AeroManage.BookingManagement.Application.Handlers.BoardingPass
{
    public class GetBoardingPassQueryHandler : IRequestHandler<GetBoardingPassQuery, ApiResponse<BoardingPassDto>>
    {
        private readonly IBoardingPassRepository _boardingPassRepo;
        private readonly ICacheService _cache;
        private readonly ILogger<GetBoardingPassQueryHandler> _logger;

        public GetBoardingPassQueryHandler(
            IBoardingPassRepository boardingPassRepo,
            ICacheService cache,
            ILogger<GetBoardingPassQueryHandler> logger)
        {
            _boardingPassRepo = boardingPassRepo;
            _cache = cache;
            _logger = logger;
        }

        public async Task<ApiResponse<BoardingPassDto>> Handle(GetBoardingPassQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var cacheKey = $"boardingpass:{request.BoardingPassId ?? 0}:{request.BoardingPassNumber ?? ""}:{request.BookingPassengerId ?? 0}";
                var cached = await _cache.GetAsync<BoardingPassDto>(cacheKey);

                if (cached != null)
                {
                    return ApiResponse<BoardingPassDto>.SuccessResponse(cached);
                }

                var boardingPass = await _boardingPassRepo.GetBoardingPassAsync(
                    request.BoardingPassId,
                    request.BoardingPassNumber,
                    request.BookingPassengerId
                );

                var result = new BoardingPassDto
                {
                    BoardingPassId = boardingPass.BoardingPassId,
                    BookingPassengerId = boardingPass.BookingPassengerId,
                    BoardingPassNumber = boardingPass.BoardingPassNumber,
                    Gate = boardingPass.Gate,
                    BoardingTime = boardingPass.BoardingTime,
                    BoardingGroup = boardingPass.BoardingGroup,
                    BoardingZone = boardingPass.BoardingZone,
                    QRCode = boardingPass.QRCode,
                    Barcode = boardingPass.Barcode,
                    IsUsed = boardingPass.IsUsed,
                    ScannedAt = boardingPass.ScannedAt,
                    GeneratedAt = boardingPass.GeneratedAt,
                    FirstName = boardingPass.FirstName,
                    LastName = boardingPass.LastName,
                    Email = boardingPass.Email,
                    SeatClass = boardingPass.SeatClass,
                    SeatNumber = boardingPass.SeatNumber,

                };

                if (boardingPass == null)
                {
                    return ApiResponse<BoardingPassDto>.ErrorResponse("Boarding pass not found");
                }

                await _cache.SetAsync(cacheKey, result, TimeSpan.FromHours(24));

                return ApiResponse<BoardingPassDto>.SuccessResponse(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving boarding pass");
                return ApiResponse<BoardingPassDto>.ErrorResponse("Failed to retrieve boarding pass", new[] { ex.Message });
            }
        }
    }
}
