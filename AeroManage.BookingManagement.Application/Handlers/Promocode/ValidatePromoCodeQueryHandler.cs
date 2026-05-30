using AeroManage.BookingManagement.Application.DTOs;
using AeroManage.BookingManagement.Application.Queries.Promocode;
using AeroManage.BookingManagement.Infrastructure.Repositories.Interfaces;
using AeroManage.Shared.Service.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AeroManage.BookingManagement.Application.Handlers.Promocode
{
    public class ValidatePromoCodeQueryHandler : IRequestHandler<ValidatePromoCodeQuery, ApiResponse<PromoCodeResultDto>>
    {
        private readonly IPromoCodeRepository _promoRepo;
        private readonly ICacheService _cache;
        private readonly ILogger<ValidatePromoCodeQueryHandler> _logger;

        public ValidatePromoCodeQueryHandler(
            IPromoCodeRepository promoRepo,
            ICacheService cache,
            ILogger<ValidatePromoCodeQueryHandler> logger)
        {
            _promoRepo = promoRepo;
            _cache = cache;
            _logger = logger;
        }

        public async Task<ApiResponse<PromoCodeResultDto>> Handle(ValidatePromoCodeQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var cacheKey = $"promo:{request.dto.Code}:{request.dto.BookingAmount}";
                var cached = await _cache.GetAsync<PromoCodeResultDto>(cacheKey);

                if (cached != null)
                {
                    return ApiResponse<PromoCodeResultDto>.SuccessResponse(cached);
                }

                var promoCode = await _promoRepo.ValidatePromoCodeAsync(request.dto.Code, request.dto.BookingAmount, cancellationToken);

                if (promoCode == null)
                {
                    return ApiResponse<PromoCodeResultDto>.ErrorResponse("Invalid promo code");
                }

                var isValid = promoCode.ValidationStatus == "Valid";
                var discountAmount = isValid ? promoCode.CalculatedDiscount : 0;

                var result = new PromoCodeResultDto
                {
                    IsValid = isValid,
                    Code = request.dto.Code,
                    ValidationMessage = promoCode.ValidationStatus,
                    DiscountAmount = discountAmount,
                    FinalAmount = request.dto.BookingAmount - discountAmount
                };

                // Cache for 1 hour
                await _cache.SetAsync(cacheKey, result, TimeSpan.FromHours(1));

                return ApiResponse<PromoCodeResultDto>.SuccessResponse(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error validating promo code {request.dto.Code}");
                return ApiResponse<PromoCodeResultDto>.ErrorResponse("Failed to validate promo code", new[] { ex.Message });
            }
        }
    }
}
