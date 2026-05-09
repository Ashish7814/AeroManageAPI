using AeroMange.Shared.Repositories;
using AeroMange.UserManagement.Application.Commands.Auth;
using AeroMange.UserManagement.Application.DTOs;
using AeroMange.UserManagement.Infrastructure.Repositories.Interfaces;
using MediatR;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace AeroMange.UserManagement.Application.Handlers.Auth
{
    public class ForgotPasswordCommandHandler : IRequestHandler<ForgotPasswordCommand, ApiResponse<bool>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IAuditLogRepository _auditLogRepository;
        private readonly IConfiguration _configuration;

        public ForgotPasswordCommandHandler(
            IUserRepository userRepository,
            IAuditLogRepository auditLogRepository,
            IConfiguration configuration)
        {
            _userRepository = userRepository;
            _auditLogRepository = auditLogRepository;
            _configuration = configuration;
        }

        public async Task<ApiResponse<bool>> Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var user = await _userRepository.GetUserByEmailAsync(request.dto.Email);

                if (user == null)
                {
                    // Return success even if user doesn't exist (security best practice)
                    return ApiResponse<bool>.SuccessResponse(
                        true,
                        "If the email exists, a password reset link has been sent"
                    );
                }

                var rawToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
                var hashedToken = HashToken(rawToken);

                //var resetToken = Guid.NewGuid().ToString();
                var tokenExpiry = DateTime.UtcNow.AddHours(1);

                await _userRepository.SetPasswordResetTokenAsync(request.dto.Email, hashedToken, tokenExpiry);

                var frontendUrl = _configuration["Frontend:BaseUrl"];
                var resetLink = $"{frontendUrl}/reset-password?token={Uri.EscapeDataString(rawToken)}";

                //await _emailService.SendEmailAsync(
                //    user.Email,
                //    "Password Reset Request",
                //    $@"
                //                <p>Hello {user.FirstName},</p>
                //                <p>You requested to reset your password.</p>
                //                <p>Click the link below to reset it:</p>
                //                <a href='{resetLink}'>Reset Password</a>
                //                <p>This link will expire in 1 hour.</p>
                //                <p>If you did not request this, please ignore this email.</p>
                //                "
                //);

                await _auditLogRepository.CreateAuditLogAsync(
                    user.UserId,
                    "PASSWORD_RESET_REQUESTED",
                    "Users",
                    user.UserId
                );

                return ApiResponse<bool>.SuccessResponse(
                    true,
                    "If the email exists, a password reset link has been sent"
                );
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.ErrorResponse(
                    "An error occurred",
                    new[] { ex.Message }
                );
            }
        }


        private string HashToken(string token)
        {
            using var sha256 = SHA256.Create();
            var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(token));
            return Convert.ToBase64String(bytes);
        }
    }
}


