using AeroManage.BookingManagement.Application.DTOs;
using AeroManage.BookingManagement.Application.Queries.Promocode;
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

namespace AeroManage.BookingManagement.Application.Handlers.Promocode
{
    public class GetPromoCodeByCodeQueryHandler : IRequestHandler<GetPromoCodeByCodeQuery, ApiResponse<PromoCodeDto>>
    {
        private readonly IPromoCodeRepository _promoRepo;
        private readonly ICacheService _cache;
        private readonly ILogger<GetActivePromoCodesQueryHandler> _logger;

        public GetPromoCodeByCodeQueryHandler(
            IPromoCodeRepository promoRepo,
            ICacheService cache,
            ILogger<GetActivePromoCodesQueryHandler> logger)
        {
            _promoRepo = promoRepo;
            _cache = cache;
            _logger = logger;
        }

        public async Task<ApiResponse<PromoCodeDto>> Handle(GetPromoCodeByCodeQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var cacheKey = "promo:code:list";
                var cached = await _cache.GetAsync<PromoCodeDto>(cacheKey);

                if (cached != null)
                {
                    return ApiResponse<PromoCodeDto>.SuccessResponse(cached);
                }
                var promoCodes = await _promoRepo.GetPromoCodeByCodeAsync(request.code);

                var result = new PromoCodeDto
                {
                    PromoCodeId = promoCodes.PromoCodeId,
                    Code = promoCodes.Code,
                    DiscountType = promoCodes.DiscountType,
                    DiscountValue = promoCodes.DiscountValue,
                    MinimumAmount = promoCodes.MinimumAmount,
                    MaximumDiscount = promoCodes.MaximumDiscount,
                    UsageLimit = promoCodes.UsageLimit,
                    UsageCount = promoCodes.UsageCount,
                    ValidFrom = promoCodes.ValidFrom,
                    ValidUntil = promoCodes.ValidUntil,
                    IsActive = promoCodes.IsActive
                };
                // Cache for 1 hour
                await _cache.SetAsync(cacheKey, result, TimeSpan.FromHours(1));

                return ApiResponse<PromoCodeDto>.SuccessResponse(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error validating promo code {request.code}");
                return ApiResponse<PromoCodeDto>.ErrorResponse("Failed to validate promo code", new[] { ex.Message });
            }
        }
    }
}
