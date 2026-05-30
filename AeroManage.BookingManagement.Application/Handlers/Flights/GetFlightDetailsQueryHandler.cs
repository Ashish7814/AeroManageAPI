using AeroManage.BookingManagement.Application.DTOs;
using AeroManage.BookingManagement.Application.Queries.Flights;
using AeroManage.BookingManagement.Infrastructure.Repositories.Interfaces;
using AeroManage.Shared.Service.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AeroManage.BookingManagement.Application.Handlers.Flights
{
    public class GetFlightDetailsQueryHandler : IRequestHandler<GetFlightDetailsQuery, ApiResponse<FlightDetailsDto>>
    {
        private readonly IBookingRepository _repo;
        private readonly ICacheService _cache;
        private readonly ILogger<GetFlightDetailsQueryHandler> _logger;

        public GetFlightDetailsQueryHandler(
            IBookingRepository repo,
            ICacheService cache,
            ILogger<GetFlightDetailsQueryHandler> logger)
        {
            _repo = repo;
            _cache = cache;
            _logger = logger;
        }

        public async Task<ApiResponse<FlightDetailsDto>> Handle(GetFlightDetailsQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var cacheKey = $"flight:details:{request.flightId}";
                var cached = await _cache.GetAsync<FlightDetailsDto>(cacheKey);

                if (cached != null)
                {
                    return ApiResponse<FlightDetailsDto>.SuccessResponse(cached);
                }

                var result = await _repo.GetFlightDetailsAsync(request.flightId);

                if (result == null)
                {
                    return ApiResponse<FlightDetailsDto>.ErrorResponse("Flight not found");
                }

                var dto = new FlightDetailsDto
                {
                    FlightId = result.FlightId,
                    FlightNumber = result.FlightNumber,
                    DepartureDateTime = result.DepartureDateTime,
                    ArrivalDateTime = result.ArrivalDateTime,
                    Duration = result.EstimatedDuration,
                    FlightStatus = result.FlightStatus,

                    Origin = new AirportInfoDto
                    {
                        AirportId = result.OriginAirportId,
                        AirportCode = result.OriginCode,
                        AirportName = result.OriginName,
                        City = result.OriginCity,
                        Country = result.OriginCountry,
                        Terminal = result.DepartureGate
                    },

                    Destination = new AirportInfoDto
                    {
                        AirportId = result.DestinationAirportId,
                        AirportCode = result.DestinationCode,
                        AirportName = result.DestinationName,
                        City = result.DestinationCity,
                        Country = result.DestinationCountry,
                        Terminal = result.ArrivalGate
                    },

                    Aircraft = new AircraftInfoDto
                    {
                        AircraftId = result.AircraftId ?? 0,
                        Model = result.AircraftModel,
                        Manufacturer = result.Manufacturer,
                        TotalSeats = result.TotalSeats ?? 0,
                        SeatsByClass = new Dictionary<string, int>()
                    },

                    SeatAvailability = new SeatAvailabilityDto
                    {
                        //seatClass = "",
                        EconomyAvailable = 0,
                        BusinessAvailable = 0,
                        FirstClassAvailable = 0,
                        TotalAvailable = 0,
                        AvailableSeats = 0
                    },

                    Pricing = new PricingBreakdownDto
                    {
                        FlightPrices = new List<FlightPriceDto>
                        {
                            new FlightPriceDto
                            {
                                FlightId = result.FlightId,
                                FlightNumber = result.FlightNumber,
                                Route = result.RouteCode,
                                EconomyPrice = result.EconomyPrice,
                                BusinessPrice = result.BusinessPrice,
                                FirstClassPrice = result.FirstClassPrice
                            }
                        }
                    },

                    Layovers = result.Layovers?.Select(l => new RouteLayoverDto
                    {
                        LayoverId = l.LayoverId,
                        RouteId = l.RouteId,
                        AirportId = l.AirportId,
                        LayoverSequence = l.LayoverSequence,
                        MinimumLayoverMinutes = l.MinimumLayoverMinutes,
                        MaximumLayoverMinutes = l.MaximumLayoverMinutes
                    }).ToList()
                };

                await _cache.SetAsync(cacheKey, dto, TimeSpan.FromMinutes(10));

                return ApiResponse<FlightDetailsDto>.SuccessResponse(dto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting flight details for {request.flightId}");
                return ApiResponse<FlightDetailsDto>.ErrorResponse("Failed to retrieve flight details", new[] { ex.Message });
            }
        }


    }
}
