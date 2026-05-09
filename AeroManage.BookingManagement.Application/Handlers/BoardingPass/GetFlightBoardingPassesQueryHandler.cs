using AeroManage.BookingManagement.Application.DTOs;
using AeroManage.BookingManagement.Application.Queries.BoardingPass;
using AeroManage.BookingManagement.Domain.Entities;
using AeroManage.BookingManagement.Infrastructure.Repositories.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AeroManage.BookingManagement.Application.Handlers.BoardingPass
{
    public class GetFlightBoardingPassesQueryHandler : IRequestHandler<GetFlightBoardingPassesQuery, ApiResponse<List<BoardingPassDto>>>
    {
        private readonly IBoardingPassRepository _boardingPassRepo;
        private readonly ILogger<GetFlightBoardingPassesQueryHandler> _logger;

        public GetFlightBoardingPassesQueryHandler(
            IBoardingPassRepository boardingPassRepo,
            ILogger<GetFlightBoardingPassesQueryHandler> logger)
        {
            _boardingPassRepo = boardingPassRepo;
            _logger = logger;
        }

        public async Task<ApiResponse<List<BoardingPassDto>>> Handle(GetFlightBoardingPassesQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var passes = await _boardingPassRepo.GetFlightBoardingPassesAsync(request.flightId);

                var result = passes.Select(p => new BoardingPassDto
                {
                    BoardingPassId = p.BoardingPassId,
                    BookingPassengerId = p.BookingPassengerId,
                    BoardingPassNumber = p.BoardingPassNumber,
                    Gate = p.Gate,
                    BoardingTime = p.BoardingTime,
                    BoardingGroup = p.BoardingGroup,
                    BoardingZone = p.BoardingZone,
                    QRCode = p.QRCode,
                    Barcode = p.Barcode,
                    IsUsed = p.IsUsed,
                    ScannedAt = p.ScannedAt,
                    GeneratedAt = p.GeneratedAt,
                    FirstName = p.FirstName,
                    LastName = p.LastName,
                    Email = p.Email,
                    SeatClass = p.SeatClass,
                    SeatNumber = p.SeatNumber,

                }).ToList();

                return ApiResponse<List<BoardingPassDto>>.SuccessResponse(result, $"Found {result.Count} boarding passes");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting boarding passes for flight {request.flightId}");
                return ApiResponse<List<BoardingPassDto>>.ErrorResponse("Failed to retrieve boarding passes", new[] { ex.Message });
            }
        }
    }
}
