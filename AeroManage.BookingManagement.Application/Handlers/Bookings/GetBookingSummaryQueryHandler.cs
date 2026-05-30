using AeroManage.BookingManagement.Application.DTOs;
using AeroManage.BookingManagement.Application.Queries.Bookings;
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

namespace AeroManage.BookingManagement.Application.Handlers.Bookings
{
    public class GetBookingSummaryQueryHandler : IRequestHandler<GetBookingSummaryQuery, ApiResponse<BookingSummaryDto>>
    {
        private readonly IBookingRepository _repo;
        private readonly ICacheService _cache;
        private readonly ILogger<GetBookingSummaryQueryHandler> _logger;

        public GetBookingSummaryQueryHandler(
            IBookingRepository repo,
            ICacheService cache,
            ILogger<GetBookingSummaryQueryHandler> logger)
        {
            _repo = repo;
            _cache = cache;
            _logger = logger;
        }

        public async Task<ApiResponse<BookingSummaryDto>> Handle(GetBookingSummaryQuery request, CancellationToken cancellationToken)
        {
            try
            {
                /*var cacheKey = $"booking:summary:{request.bookingId}";
                var cached = await _cache.GetAsync<BookingSummaryDto>(cacheKey);

                if (cached != null)
                {
                    return ApiResponse<BookingSummaryDto>.SuccessResponse(cached);
                }

                var result = await _repo.GetBookingSummaryAsync(request.bookingId);

                if (result == null)
                {
                    return ApiResponse<BookingSummaryDto>.ErrorResponse("Booking not found");
                }

                var dto = new BookingSummaryDto
                {
                    BookingId = result.BookingId,
                    BookingReference = result.BookingReference,
                    PNR = result.PNR,
                    BookingStatus = result.BookingStatus,
                    TotalAmount = result.TotalAmount,
                    Currency = result.Currency,
                    PaymentStatus = result.PaymentStatus,
                    BookingDate = result.BookingDate,
                    ContactEmail = result.BookingEmail,
                    PassengerCount = result.Passengers?.Count ?? 0,
                    FlightCount = result.Flights?.Count ?? 0,

                    Flights = result.Flights?.Select(f => new FlightSummaryDto
                    {
                        FlightId = f.FlightId,
                        FlightNumber = f.FlightNumber,
                        DepartureDateTime = f.DepartureDateTime,
                        ArrivalDateTime = f.ArrivalDateTime,
                        OriginCode = f.OriginCode,
                        DestinationCode = f.DestinationCode,
                        Status = f.FlightStatus
                    }).ToList(),

                    Pricing = result.Pricing == null ? null : new BookingPricingDto
                    {
                        BookingId = result.BookingId,
                        BasePrice = result.Pricing.BasePrice,
                        TaxAmount = result.Pricing.TaxAmount,
                        ServiceFee = result.Pricing.ServiceFee,
                        BaggageFee = result.Pricing.BaggageFee,
                        SeatSelectionFee = result.Pricing.SeatSelectionFee,
                        InsuranceFee = result.Pricing.InsuranceFee,
                        DiscountAmount = result.Pricing.DiscountAmount,
                        PromoCode = result.Pricing.PromoCode,
                        TotalAmount = result.Pricing.TotalAmount,
                        Currency = result.Currency
                    },

                    PaymentInfo = new PaymentStatusDto
                    {
                        PaymentStatus = result.PaymentStatus,
                        Amount = result.TotalAmount,
                        Currency = result.Currency,
                        PaymentMethod = null,
                        PaymentDate = null,
                        TransactionId = null
                    }
                };

                await _cache.SetAsync(cacheKey, dto, TimeSpan.FromMinutes(1));

                return ApiResponse<BookingSummaryDto>.SuccessResponse(dto);*/



                var data = new BookingSummaryDto
                {
                    BookingId = 1234,
                    BookingReference = "BK-2026-001",
                    PNR = "PNR78945",
                    BookingStatus = "Confirmed",
                    TotalAmount = 5200,
                    Currency = "INR",
                    PaymentStatus = "Paid",
                    BookingDate = DateTime.Now.AddDays(-1),
                    ContactEmail = "testuser@mail.com",
                    PassengerCount = 2,
                    FlightCount = 1,

                    Flights = new List<FlightSummaryDto>
        {
            new FlightSummaryDto
            {
                FlightId = 1,
                FlightNumber = "AI-202",
                DepartureDateTime = DateTime.Now.AddHours(5),
                ArrivalDateTime = DateTime.Now.AddHours(8),
                AirlineCode = "DEL",
                // = "Delhi",
                //DestinationCode = "BOM",
                //DestinationCity = "Mumbai",
                //DestinationCountry = "India",
                Status = "Scheduled"
            }
        },

                    Pricing = new BookingPricingDto
                    {
                        PricingId = 1,
                        BookingId = 1234,
                        BasePrice = 4000,
                        TaxAmount = 800,
                        ServiceFee = 200,
                        BaggageFee = 100,
                        SeatSelectionFee = 50,
                        InsuranceFee = 50,
                        DiscountAmount = 0,
                        PromoCode = null,
                        TotalAmount = 5200,
                        Currency = "INR"
                    },

                    PaymentInfo = new PaymentStatusDto
                    {
                        PaymentStatus = "Paid",
                        Amount = 5200,
                        Currency = "INR",
                        PaymentMethod = "Credit Card",
                        PaymentDate = DateTime.Now.AddHours(-2),
                        TransactionId = "TXN123456"
                    }
                };

                return ApiResponse<BookingSummaryDto>.SuccessResponse(data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting booking summary for {request.bookingId}");
                return ApiResponse<BookingSummaryDto>.ErrorResponse("Failed to retrieve booking summary", new[] { ex.Message });
            }
        }
    }
}
