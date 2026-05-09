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
    public class GetActivePromoCodesQueryHandler : IRequestHandler<GetActivePromoCodesQuery, ApiResponse<List<PromoCodeDto>>>
    {
        private readonly IPromoCodeRepository _promoRepo;
        private readonly ICacheService _cache;
        private readonly ILogger<GetActivePromoCodesQueryHandler> _logger;

        public GetActivePromoCodesQueryHandler(
            IPromoCodeRepository promoRepo,
            ICacheService cache,
            ILogger<GetActivePromoCodesQueryHandler> logger)
        {
            _promoRepo = promoRepo;
            _cache = cache;
            _logger = logger;
        }

        public async Task<ApiResponse<List<PromoCodeDto>>> Handle(GetActivePromoCodesQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var cacheKey = "promo:active:list";
                var cached = await _cache.GetAsync<List<PromoCodeDto>>(cacheKey);

                if (cached != null)
                {
                    return ApiResponse<List<PromoCodeDto>>.SuccessResponse(cached);
                }

                var promoCodes = await _promoRepo.GetActivePromoCodesAsync();

                var dtos = promoCodes.Select(p => new PromoCodeDto
                {
                    PromoCodeId = p.PromoCodeId,
                    Code = p.Code,
                    DiscountType = p.DiscountType,
                    DiscountValue = p.DiscountValue,
                    MinimumAmount = p.MinimumAmount,
                    MaximumDiscount = p.MaximumDiscount,
                    UsageLimit = p.UsageLimit,
                    UsageCount = p.UsageCount,
                    ValidFrom = p.ValidFrom,
                    ValidUntil = p.ValidUntil,
                    IsActive = p.IsActive
                }).ToList();

                // Cache for 1 hour
                await _cache.SetAsync(cacheKey, dtos, TimeSpan.FromHours(1));

                return ApiResponse<List<PromoCodeDto>>.SuccessResponse(dtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting active promo codes");
                return ApiResponse<List<PromoCodeDto>>.ErrorResponse("Failed to retrieve promo codes", new[] { ex.Message });
            }
        }
    }
}
