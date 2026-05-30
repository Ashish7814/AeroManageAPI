using AeroManage.FlightManagement.Application.DTOs;
using AeroManage.FlightManagement.Application.Queries.Routes;
using AeroManage.FlightManagement.Infrastructure.Repositories.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AeroManage.FlightManagement.Application.Handlers.Routes
{
    public class GetRouteByIdQueryHandler : IRequestHandler<GetRouteByIdQuery, ApiResponse<RouteDto>>
    {
        private readonly IRouteRepository _repository;

        public GetRouteByIdQueryHandler(IRouteRepository repository)
        {
            _repository = repository;
        }

        public async Task<ApiResponse<RouteDto>> Handle(GetRouteByIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var route = await _repository.GetRouteByIdAsync(request.routeId);

                if (route == null)
                {
                    return ApiResponse<RouteDto>.ErrorResponse("Route not found");
                }

                var dto = new RouteDto
                {
                    RouteId = route.RouteId,
                    RouteCode = route.RouteCode,
                    OriginAirportId = route.OriginAirportId,
                    DestinationAirportId = route.DestinationAirportId,
                    Distance = route.Distance,
                    EstimatedDuration = route.EstimatedDuration,
                    //OriginCode = route.OriginCode,
                    //OriginName = route.OriginName,
                    //OriginCity = route.OriginCity,
                    //OriginCountry = route.OriginCountry,
                    //DestinationCode = route.DestinationCode,
                    //DestinationName = route.DestinationName,
                    //DestinationCity = route.DestinationCity,
                    //DestinationCountry = route.DestinationCountry
                };

                return ApiResponse<RouteDto>.SuccessResponse(dto);
            }
            catch (Exception ex)
            {
                return ApiResponse<RouteDto>.ErrorResponse("Error fetching route", new[] { ex.Message });
            }
        }
    }
}
