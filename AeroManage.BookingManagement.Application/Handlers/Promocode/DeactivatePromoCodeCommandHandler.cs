using AeroManage.BookingManagement.Application.Commands.Promocode;
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
    public class DeactivatePromoCodeCommandHandler : IRequestHandler<DeactivatePromoCodeCommand, ApiResponse<bool>>
    {
        private readonly IPromoCodeRepository _promoRepo;
        private readonly ICacheService _cache;
        private readonly ILogger<GetActivePromoCodesQueryHandler> _logger;

        public DeactivatePromoCodeCommandHandler(
            IPromoCodeRepository promoRepo,
            ICacheService cache,
            ILogger<GetActivePromoCodesQueryHandler> logger)
        {
            _promoRepo = promoRepo;
            _cache = cache;
            _logger = logger;
        }
        public async Task<ApiResponse<bool>> Handle(DeactivatePromoCodeCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var cacheKey = "promo:active:list";
                var cached = await _cache.GetAsync<List<PromoCodeDto>>(cacheKey);

                var promocodes = await _promoRepo.DeactivatePromoCodeAsync(request.promoCodeId);
             /*   await _cache.RemoveAsync("promo:active:{result}");*/

                return ApiResponse<bool>.SuccessResponse(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting deactivate promo codes");
                return ApiResponse<bool>.ErrorResponse("Internal server error");
            }
        }
    }
}
