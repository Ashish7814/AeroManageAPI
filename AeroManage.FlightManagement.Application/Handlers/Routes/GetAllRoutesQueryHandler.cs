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
    public class GetAllRoutesQueryHandler : IRequestHandler<GetAllRoutesQuery, ApiResponse<PagedResultDto<RouteDto>>>
    {
        private readonly IRouteRepository _repository;

        public GetAllRoutesQueryHandler(IRouteRepository repository)
        {
            _repository = repository;
        }

        public async Task<ApiResponse<PagedResultDto<RouteDto>>> Handle(GetAllRoutesQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var (routes, totalRecords) = await _repository.GetAllRoutesAsync(
                    request.dto.PageNumber,
                    request.dto.PageSize);

                var dtos = routes.Select(r => new RouteDto
                {
                    RouteId = r.RouteId,
                    RouteCode = r.RouteCode,
                    OriginAirportId = r.OriginAirportId,
                    DestinationAirportId = r.DestinationAirportId,
                    Distance = r.Distance,
                    EstimatedDuration = r.EstimatedDuration,
                    //OriginCode = r.OriginCode,
                    //OriginName = r.OriginName,
                    //OriginCity = r.OriginCity,
                    //OriginCountry = r.OriginCountry,
                    //DestinationCode = r.DestinationCode,
                    //DestinationName = r.DestinationName,
                    //DestinationCity = r.DestinationCity,
                    //DestinationCountry = r.DestinationCountry
                }).ToArray();

                var result = new PagedResultDto<RouteDto>
                {
                    PageNumber = request.dto.PageNumber,
                    PageSize = request.dto.PageSize,
                    TotalRecords = totalRecords,
                    Data = dtos
                };

                return ApiResponse<PagedResultDto<RouteDto>>.SuccessResponse(result);
            }
            catch (Exception ex)
            {
                return ApiResponse<PagedResultDto<RouteDto>>.ErrorResponse("Error fetching routes", new[] { ex.Message });
            }
        }
    }
}
