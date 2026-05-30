using AeroManage.BookingManagement.Application.Commands.Promocode;
using AeroManage.BookingManagement.Application.DTOs;
using AeroManage.BookingManagement.Domain.Entities;
using AeroManage.BookingManagement.Infrastructure.Repositories.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AeroManage.BookingManagement.Application.Handlers.Promocode
{
    public class CreatePromoCodeCommandHandler : IRequestHandler<CreatePromoCodeCommand, ApiResponse<PromoCodeDto>>
    {
        private readonly IPromoCodeRepository _promoRepo;
        private readonly ILogger<CreatePromoCodeCommandHandler> _logger;

        public CreatePromoCodeCommandHandler(
            IPromoCodeRepository promoRepo,
            ILogger<CreatePromoCodeCommandHandler> logger)
        {
            _promoRepo = promoRepo;
            _logger = logger;
        }

        public async Task<ApiResponse<PromoCodeDto>> Handle(CreatePromoCodeCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var promoCode = new PromoCode
                {
                    Code = request.dto.Code.ToUpper(),
                    DiscountType = request.dto.DiscountType,
                    DiscountValue = request.dto.DiscountValue,
                    MinimumAmount = request.dto.MinimumAmount,
                    MaximumDiscount = request.dto.MaximumDiscount,
                    UsageLimit = request.dto.UsageLimit,
                    ValidFrom = request.dto.ValidFrom,
                    ValidUntil = request.dto.ValidUntil,
                    IsActive = true
                };

                var promoCodeId = await _promoRepo.CreatePromoCodeAsync(promoCode);
                promoCode.PromoCodeId = promoCodeId;

                var dto = new PromoCodeDto
                {
                    PromoCodeId = promoCodeId,
                    Code = promoCode.Code,
                    DiscountType = promoCode.DiscountType,
                    DiscountValue = promoCode.DiscountValue,
                    MinimumAmount = promoCode.MinimumAmount,
                    MaximumDiscount = promoCode.MaximumDiscount,
                    UsageLimit = promoCode.UsageLimit,
                    ValidFrom = promoCode.ValidFrom,
                    ValidUntil = promoCode.ValidUntil,
                    IsActive = true
                };

                return ApiResponse<PromoCodeDto>.SuccessResponse(dto, "Promo code created successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating promo code");
                return ApiResponse<PromoCodeDto>.ErrorResponse("Failed to create promo code", new[] { ex.Message });
            }
        }
    }
}
