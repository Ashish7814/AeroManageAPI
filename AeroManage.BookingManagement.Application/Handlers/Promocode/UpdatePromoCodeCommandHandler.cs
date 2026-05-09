using AeroManage.BookingManagement.Application.Commands.Promocode;
using AeroManage.BookingManagement.Application.DTOs;
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
    public class UpdatePromoCodeCommandHandler : IRequestHandler<UpdatePromoCodeCommand, ApiResponse<bool>>
    {
        private readonly IPromoCodeRepository _promoRepo;
        private readonly ICacheService _cache;
        private readonly ILogger<GetActivePromoCodesQueryHandler> _logger;

        public UpdatePromoCodeCommandHandler(
            IPromoCodeRepository promoRepo,
            ICacheService cache,
            ILogger<GetActivePromoCodesQueryHandler> logger)
        {
            _promoRepo = promoRepo;
            _cache = cache;
            _logger = logger;
        }

        public async Task<ApiResponse<bool>> Handle(UpdatePromoCodeCommand request, CancellationToken cancellationToken)
        {
            try
            {
                

                var procodes = await _promoRepo.GetPromoCodeByCodeAsync(request.dto.Code);
                if (procodes == null)
                    return null;

                var promoCode = new PromoCode
                {
                    Code = procodes.Code,
                    DiscountType = procodes.DiscountType,
                    DiscountValue = procodes.DiscountValue,
                    MinimumAmount = procodes.MinimumAmount,
                    MaximumDiscount = procodes.MaximumDiscount,
                    UsageLimit = procodes.UsageLimit,
                    UsageCount = procodes.UsageCount,
                    ValidFrom = procodes.ValidFrom,
                    ValidUntil = procodes.ValidUntil,
                    IsActive = procodes.IsActive,
                    CreatedAt = procodes.CreatedAt,
                    ValidationStatus = procodes.ValidationStatus,
                    CalculatedDiscount = procodes.CalculatedDiscount,
                };

                var result = await _promoRepo.UpdatePromoCodeAsync(promoCode);
                return ApiResponse<bool>.SuccessResponse(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting updating promo codes");
                return ApiResponse<bool>.ErrorResponse("Internal server error");
            }
        }
    }
}
