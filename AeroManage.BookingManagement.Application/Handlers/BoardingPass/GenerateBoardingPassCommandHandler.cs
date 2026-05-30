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
    public class GenerateBoardingPassCommandHandler : IRequestHandler<GenerateBoardingPassCommand, ApiResponse<BoardingPassDto>>
    {
        private readonly IBoardingPassRepository _boardingPassRepo;
        private readonly ILogger<GenerateBoardingPassCommandHandler> _logger;

        public GenerateBoardingPassCommandHandler(
            IBoardingPassRepository boardingPassRepo,
            ILogger<GenerateBoardingPassCommandHandler> logger)
        {
            _boardingPassRepo = boardingPassRepo;
            _logger = logger;
        }

        public async Task<ApiResponse<BoardingPassDto>> Handle(GenerateBoardingPassCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var (boardingPassId, boardingPassNumber, qrCode, boardingGroup, boardingZone) =
                    await _boardingPassRepo.GenerateBoardingPassAsync(
                        request.dto.BookingPassengerId,
                        request.dto.Gate,
                        request.dto.BoardingTime,
                        request.dto.BoardingGroup,
                        request.dto.BoardingZone
                    );

                var result = new BoardingPassDto
                {
                    BoardingPassId = boardingPassId,
                    BoardingPassNumber = boardingPassNumber,
                    QRCode = qrCode,
                    BoardingGroup = boardingGroup,
                    BoardingZone = boardingZone,
                    Gate = request.dto.Gate,
                    BoardingTime = request.dto.BoardingTime
                };

                return ApiResponse<BoardingPassDto>.SuccessResponse(result, "Boarding pass generated successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating boarding pass");
                return ApiResponse<BoardingPassDto>.ErrorResponse("Failed to generate boarding pass", new[] { ex.Message });
            }
        }
    }
}
