using AeroManage.BookingManagement.Application.Commands.Bookings;
using AeroManage.BookingManagement.Application.DTOs;
using AeroManage.BookingManagement.Infrastructure.Repositories.Interfaces;
using AeroManage.Shared.Service.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AeroManage.BookingManagement.Application.Handlers.Bookings
{
    public class AddBookingAddonsCommandHandler : IRequestHandler<AddBookingAddonsCommand, ApiResponse<BookingPricingDto>>
    {
        private readonly IBookingRepository _repo;
        private readonly IBookingPricingRepository _pricingRepo;
        private readonly ICacheService _cache;
        private readonly ILogger<AddBookingAddonsCommandHandler> _logger;

        public AddBookingAddonsCommandHandler(
            IBookingRepository repo,
            IBookingPricingRepository pricingRepo,
            ICacheService cache,
            ILogger<AddBookingAddonsCommandHandler> logger)
        {
            _repo = repo ?? throw new ArgumentNullException(nameof(repo));
            _pricingRepo = pricingRepo;
            _cache = cache;
            _logger = logger;
        }

        public async Task<ApiResponse<BookingPricingDto>> Handle(AddBookingAddonsCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var addonsCost = await _repo.AddBookingAddonAsync(
                    request.dto.PassengerId,
                    request.dto.ExtraBaggage,
                    request.dto.TravelInsurance,
                    request.dto.PriorityBoarding,
                    request.dto.LoungeAccess
                );

                var pricing = await _pricingRepo.GetPricingByBookingIdAsync(request.bookingId);

                var result = new BookingPricingDto
                {
                    PricingId = pricing.PricingId,
                    BookingId = pricing.BookingId,
                    BasePrice = pricing.BasePrice,
                    TaxAmount = pricing.TaxAmount,
                    ServiceFee = pricing.ServiceFee,
                    BaggageFee = pricing.BaggageFee,
                    SeatSelectionFee = pricing.SeatSelectionFee,
                    InsuranceFee = pricing.InsuranceFee,
                    DiscountAmount = pricing.DiscountAmount,
                    PromoCode = pricing.PromoCode,
                    TotalAmount = pricing.TotalAmount,
                    Currency = pricing.Currency
                };

                await _cache.RemoveAsync($"booking:summary:{request.bookingId}");
                await _cache.RemoveAsync($"booking:{request.bookingId}");

                return ApiResponse<BookingPricingDto>.SuccessResponse(result, $"Add-ons added. Additional cost: ${addonsCost:F2}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding booking addons");
                return ApiResponse<BookingPricingDto>.ErrorResponse("Failed to add addons", new[] { ex.Message });
            }
        }
    }
}
