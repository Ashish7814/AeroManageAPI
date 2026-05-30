using AeroManage.UserManagement.Domain.Entities;
using AeroMange.Shared.Repositories;
using AeroMange.UserManagement.Application.Commands.Auth;
using AeroMange.UserManagement.Application.DTOs;
using AeroMange.UserManagement.Application.Service;
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
    public class ResetPasswordCommandHandler : IRequestHandler<ResetPasswordCommand, ApiResponse<bool>>
    {
        private readonly IUserRepository _userRepository;

        public ResetPasswordCommandHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<ApiResponse<bool>> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var (hash, salt) = PasswordHasher.HashPassword(request.dto.NewPassword);

                var result = await _userRepository.ResetPasswordAsync(request.dto.Token, hash, salt);

                if (result)
                {
                    return ApiResponse<bool>.SuccessResponse(true, "Password reset successfully");
                }

                return ApiResponse<bool>.ErrorResponse("Invalid or expired reset token");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.ErrorResponse(
                    "An error occurred during password reset",
                    new[] { ex.Message }
                );
            }
        }
    }
}






//using System.Security.Cryptography;
//using System.Text;

//public class ForgotPasswordCommandHandler : IRequestHandler<ForgotPasswordCommand, ApiResponse<bool>>
//{
//    private readonly IUserRepository _userRepository;
//    private readonly IEmailService _emailService;
//    private readonly IConfiguration _configuration;
//    private readonly IAuditLogRepository _auditLogRepository;

//    public ForgotPasswordCommandHandler(
//        IUserRepository userRepository,
//        IEmailService emailService,
//        IConfiguration configuration,
//        IAuditLogRepository auditLogRepository)
//    {
//        _userRepository = userRepository;
//        _emailService = emailService;
//        _configuration = configuration;
//        _auditLogRepository = auditLogRepository;
//    }

//    public async Task<ApiResponse<bool>> Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
//    {
//        try
//        {
//            var user = await _userRepository.GetUserByEmailAsync(request.dto.Email);

//            // Always return success (prevent email enumeration)
//            if (user == null)
//            {
//                return ApiResponse<bool>.SuccessResponse(
//                    true,
//                    "If the email exists, a password reset link has been sent"
//                );
//            }

//            // Generate secure random token
//            var rawToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
//            var hashedToken = HashToken(rawToken);

//            var expiry = DateTime.UtcNow.AddHours(1);

//            await _userRepository.SetPasswordResetTokenAsync(
//                user.Email,
//                hashedToken,
//                expiry
//            );

//            var frontendUrl = _configuration["Frontend:BaseUrl"];
//            var resetLink = $"{frontendUrl}/reset-password?token={Uri.EscapeDataString(rawToken)}";

//            await _emailService.SendEmailAsync(
//                user.Email,
//                "Password Reset Request",
//                $@"
//                <p>Hello {user.FirstName},</p>
//                <p>You requested to reset your password.</p>
//                <p>Click the link below to reset it:</p>
//                <a href='{resetLink}'>Reset Password</a>
//                <p>This link will expire in 1 hour.</p>
//                <p>If you did not request this, please ignore this email.</p>
//                "
//            );

//            await _auditLogRepository.CreateAuditLogAsync(
//                user.UserId,
//                "PASSWORD_RESET_REQUESTED",
//                "Users",
//                user.UserId
//            );

//            return ApiResponse<bool>.SuccessResponse(
//                true,
//                "If the email exists, a password reset link has been sent"
//            );
//        }
//        catch (Exception ex)
//        {
//            return ApiResponse<bool>.ErrorResponse(
//                "An error occurred",
//                new[] { ex.Message }
//            );
//        }
//    }

//    private string HashToken(string token)
//    {
//        using var sha256 = SHA256.Create();
//        var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(token));
//        return Convert.ToBase64String(bytes);
//    }
//}

//using System.Security.Cryptography;
//using System.Text;

//public class ResetPasswordCommandHandler : IRequestHandler<ResetPasswordCommand, ApiResponse<bool>>
//{
//    private readonly IUserRepository _userRepository;

//    public ResetPasswordCommandHandler(IUserRepository userRepository)
//    {
//        _userRepository = userRepository;
//    }

//    public async Task<ApiResponse<bool>> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
//    {
//        try
//        {
//            if (string.IsNullOrWhiteSpace(request.dto.Token))
//            {
//                return ApiResponse<bool>.ErrorResponse("Invalid reset token");
//            }

//            var hashedToken = HashToken(request.dto.Token);

//            var user = await _userRepository.GetUserByResetTokenAsync(hashedToken);

//            if (user == null || user.ResetTokenExpiry < DateTime.UtcNow)
//            {
//                return ApiResponse<bool>.ErrorResponse("Invalid or expired reset token");
//            }

//            var (hash, salt) = PasswordHasher.HashPassword(request.dto.NewPassword);

//            await _userRepository.UpdatePasswordAsync(
//                user.UserId,
//                hash,
//                salt
//            );

//            // Clear reset token after successful use
//            await _userRepository.ClearResetTokenAsync(user.UserId);

//            return ApiResponse<bool>.SuccessResponse(
//                true,
//                "Password reset successfully"
//            );
//        }
//        catch (Exception ex)
//        {
//            return ApiResponse<bool>.ErrorResponse(
//                "An error occurred during password reset",
//                new[] { ex.Message }
//            );
//        }
//    }

//    private string HashToken(string token)
//    {
//        using var sha256 = SHA256.Create();
//        var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(token));
//        return Convert.ToBase64String(bytes);
//    }
//}

//Task SetPasswordResetTokenAsync(string email, string hashedToken, DateTime expiry);

//Task<User> GetUserByResetTokenAsync(string hashedToken);

//Task UpdatePasswordAsync(Guid userId, string passwordHash, string salt);

//Task ClearResetTokenAsync(Guid userId);
