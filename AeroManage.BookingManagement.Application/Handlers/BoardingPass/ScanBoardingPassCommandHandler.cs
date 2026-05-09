using AeroManage.BookingManagement.Application.Commands.BoardingPass;
using AeroManage.BookingManagement.Application.DTOs;
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
    public class ScanBoardingPassCommandHandler : IRequestHandler<ScanBoardingPassCommand, ApiResponse<BoardingPassScanResultDto>>
    {
        private readonly IBoardingPassRepository _boardingPassRepo;
        private readonly ILogger<ScanBoardingPassCommandHandler> _logger;

        public ScanBoardingPassCommandHandler(
            IBoardingPassRepository boardingPassRepo,
            ILogger<ScanBoardingPassCommandHandler> logger)
        {
            _boardingPassRepo = boardingPassRepo;
            _logger = logger;
        }

        public async Task<ApiResponse<BoardingPassScanResultDto>> Handle(ScanBoardingPassCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var boardingPassScan = await _boardingPassRepo.ScanBoardingPassAsync(
                    request.BoardingPassNumber,
                    request.ScannedBy
                );

                var result = new BoardingPassScanResultDto
                {
                    Success = boardingPassScan.Success,
                    Message = boardingPassScan.Message,
                    FirstName = boardingPassScan.FirstName,
                    LastName = boardingPassScan.LastName,
                    SeatNumber = boardingPassScan.SeatNumber,
                    SeatClass = boardingPassScan.SeatClass,
                    FlightNumber = boardingPassScan.FlightNumber,
                    BoardingGroup = boardingPassScan.BoardingGroup,
                    Gate = boardingPassScan.Gate,
                };

                if (!boardingPassScan.Success)
                {
                    return ApiResponse<BoardingPassScanResultDto>.ErrorResponse(result.Message);
                }

                return ApiResponse<BoardingPassScanResultDto>.SuccessResponse(result, "Boarding pass scanned successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error scanning boarding pass");
                return ApiResponse<BoardingPassScanResultDto>.ErrorResponse("Failed to scan boarding pass", new[] { ex.Message });
            }
        }
    }

}
