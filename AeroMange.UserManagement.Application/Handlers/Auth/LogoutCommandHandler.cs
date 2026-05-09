using AeroMange.UserManagement.Application.Commands.Auth;
using AeroMange.UserManagement.Application.DTOs;
using AeroMange.UserManagement.Infrastructure.Repositories.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AeroMange.UserManagement.Application.Handlers.Auth
{
    public class LogoutCommandHandler : IRequestHandler<LogoutCommand, ApiResponse<bool>>
    {
        private readonly IRefreshTokenRepository _refreshTokenRepository;

        public LogoutCommandHandler(IRefreshTokenRepository refreshTokenRepository)
        {
            _refreshTokenRepository = refreshTokenRepository;
        }

        public async Task<ApiResponse<bool>> Handle(LogoutCommand request, CancellationToken cancellationToken)
        {
            try
            {
                await _refreshTokenRepository.RevokeRefreshTokenAsync(request.dto.RefreshToken);
                return ApiResponse<bool>.SuccessResponse(true, "Logged out successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.ErrorResponse(
                    "An error occurred during logout",
                    new[] { ex.Message }
                );
            }
        }
    }
}
