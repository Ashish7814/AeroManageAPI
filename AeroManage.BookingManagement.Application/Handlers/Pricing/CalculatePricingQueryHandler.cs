using AeroManage.BookingManagement.Application.DTOs;
using AeroManage.BookingManagement.Application.Queries.Pricing;
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

namespace AeroManage.BookingManagement.Application.Handlers.Pricing
{
    public class CalculatePricingQueryHandler : IRequestHandler<CalculatePricingQuery, PricingCalculationResponseDto>
    {
        private readonly IBookingPricingRepository _repository;
        private readonly ICacheService _cacheService;
        private readonly ILogger<CalculatePricingQueryHandler> _logger;

        public CalculatePricingQueryHandler(
            IBookingPricingRepository repository,
            ICacheService cacheService,
            ILogger<CalculatePricingQueryHandler> logger)
        {
            _repository = repository;
            _cacheService = cacheService;
            _logger = logger;
        }

        public async Task<PricingCalculationResponseDto> Handle(CalculatePricingQuery request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Calculating pricing for {PassengerCount} passengers on {FlightCount} flights",
                    request.dto.Passengers.Count,
                    request.dto.FlightIds.Count);

                // Generate cache key based on request parameters
                var cacheKey = GenerateCacheKey(request.dto);

                // Try get from cache (valid for 5 minutes since prices can fluctuate)
                var cached = await _cacheService.GetAsync<PricingCalculationResponseDto>(cacheKey);
                if (cached != null)
                {
                    _logger.LogInformation("Pricing calculation served from cache");
                    return cached;
                }

                // Calculate pricing
                /*var pricing = await _repository.CalculatePricingAsync(request.dto);*/


                var domainRequest = new PricingCalculation
                {
                    FlightIds = request.dto.FlightIds,
                    Passengers = request.dto.Passengers.Select(p => new PassengerPricing
                    {
                        PassengerNumber = p.PassengerNumber,
                        PassengerType = p.PassengerType,
                        SeatClass = p.SeatClass,
                        BasePrice = p.BasePrice,
                        DiscountApplied = p.DiscountApplied,
                        FinalPrice = p.FinalPrice,
                        ExtraBaggage = p.ExtraBaggage,
                        TravelInsurance = p.TravelInsurance
                    }).ToList(),
                    PromoCode = request.dto.PromoCode
                };

                // Call repository with domain entity
                var result = await _repository.CalculatePricingAsync(domainRequest);

                // ====== Map Domain → DTO ======
                var response = new PricingCalculationResponseDto
                {
                    BasePrice = result.BasePrice,
                    TaxAmount = result.TaxAmount,
                    ServiceFee = result.ServiceFee,
                    BaggageFee = result.BaggageFee,
                    SeatSelectionFee = result.SeatSelectionFee,
                    InsuranceFee = result.InsuranceFee,
                    Subtotal = result.Subtotal,
                    DiscountAmount = result.DiscountAmount,
                    PromoCode = result.PromoCode,
                    TotalAmount = result.TotalAmount,
                    Currency = result.Currency,
                    Breakdown = new PricingBreakdownDto
                    {
                        FlightPrices = result.Breakdown.FlightPrices.Select(f => new FlightPriceDto
                        {
                            FlightId = f.FlightId,
                            FlightNumber = f.FlightNumber,
                            Route = f.Route,
                            BasePrice = f.BasePrice,
                            EconomyPrice = f.EconomyPrice,
                            BusinessPrice = f.BusinessPrice,
                            FirstClassPrice = f.FirstClassPrice
                        }).ToList(),

                        PassengerPrices = result.Breakdown.PassengerPrices.Select(p => new PassengerPriceDto
                        {
                            PassengerNumber = p.PassengerNumber,
                            PassengerType = p.PassengerType,
                            SeatClass = p.SeatClass,
                            BasePrice = p.BasePrice,
                            DiscountApplied = p.DiscountApplied,
                            FinalPrice = p.FinalPrice
                        }).ToList(),

                        Fees = result.Breakdown.Fees.Select(f => new FeeLineItemDto
                        {
                            Name = f.Name,
                            Description = f.Description,
                            Amount = f.Amount
                        }).ToList()
                    }
                };

                // Cache the result for 5 minutes
                await _cacheService.SetAsync(cacheKey, response, TimeSpan.FromMinutes(5));

                _logger.LogInformation("Pricing calculated successfully. Total: ${Total}", response.TotalAmount);

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calculating pricing");
                throw;
            }
        }

        private string GenerateCacheKey(PricingCalculationRequestDto request)
        {
            var flightIds = string.Join(",", request.FlightIds);
            var passengers = string.Join("|", request.Passengers.Select(p => $"{p.PassengerType}-{p.SeatClass}"));
            return $"Pricing:{flightIds}:{passengers}:{request.ExtraBaggageCount}:{request.TravelInsurance}:{request.PromoCode}";
        }
    }
}
