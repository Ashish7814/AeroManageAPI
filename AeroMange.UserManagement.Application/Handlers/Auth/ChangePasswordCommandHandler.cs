using AeroMange.Shared.Repositories;
using AeroMange.UserManagement.Application.Commands.Auth;
using AeroMange.UserManagement.Application.DTOs;
using AeroMange.UserManagement.Application.Service;
using AeroMange.UserManagement.Infrastructure.Repositories.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AeroMange.UserManagement.Application.Handlers.Auth
{
    public class ChangePasswordCommandHandler : IRequestHandler<ChangePasswordCommand, ApiResponse<bool>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly IAuditLogRepository _auditLogRepository;

        public ChangePasswordCommandHandler(
            IUserRepository userRepository,
            IRefreshTokenRepository refreshTokenRepository,
            IAuditLogRepository auditLogRepository)
        {
            _userRepository = userRepository;
            _refreshTokenRepository = refreshTokenRepository;
            _auditLogRepository = auditLogRepository;
        }

        public async Task<ApiResponse<bool>> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var user = await _userRepository.GetUserByIdAsync(request.userId);

                if (user == null)
                {
                    return ApiResponse<bool>.ErrorResponse("User not found");
                }

                // Verify current password
                if (!PasswordHasher.VerifyPassword(request.dto.CurrentPassword, user.PasswordHash, user.PasswordSalt))
                {
                    return ApiResponse<bool>.ErrorResponse("Current password is incorrect");
                }

                // Hash new password
                var (hash, salt) = PasswordHasher.HashPassword(request.dto.NewPassword);

                // Create a temporary reset token
                var resetToken = Guid.NewGuid().ToString();
                var tokenExpiry = DateTime.UtcNow.AddMinutes(5);

                await _userRepository.SetPasswordResetTokenAsync(user.Email, resetToken, tokenExpiry);
                await _userRepository.ResetPasswordAsync(resetToken, hash, salt);

                // Revoke all refresh tokens
                await _refreshTokenRepository.RevokeAllUserTokensAsync(request.dto.UserId);

                await _auditLogRepository.CreateAuditLogAsync(
                    request.dto.UserId,
                    "PASSWORD_CHANGED",
                    "Users",
                    request.dto.UserId
                );

                return ApiResponse<bool>.SuccessResponse(true, "Password changed successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.ErrorResponse(
                    "An error occurred during password change",
                    new[] { ex.Message }
                );
            }
        }
    }
}
