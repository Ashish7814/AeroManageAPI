using AeroManage.FlightManagement.Domain.Entities;
using AeroManage.FlightManagement.Domain.Interfaces;
using AeroManage.FlightManagement.Infrastructure.Repositories.Interfaces;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AeroManage.FlightManagement.Infrastructure.Repositories.Implementation
{
    public class RouteRepository : IRouteRepository
    {
        private readonly IDapperUnitOfWork _unitOfWork;

        public RouteRepository(IDapperUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Route> CreateRouteAsync(Route route)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@RouteCode", route.RouteCode);
            parameters.Add("@OriginAirportId", route.OriginAirportId);
            parameters.Add("@DestinationAirportId", route.DestinationAirportId);
            parameters.Add("@Distance", route.Distance);
            parameters.Add("@EstimatedDuration", route.EstimatedDuration);

            var result = await _unitOfWork.Connection.QueryFirstOrDefaultAsync<Route>(
                "sp_CreateRoute",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return result;
        }

        public async Task<Route> GetRouteByIdAsync(int routeId)
        {
            var result = await _unitOfWork.Connection.QueryFirstOrDefaultAsync<Route>(
                "sp_GetRouteById",
                new { RouteId = routeId },
                commandType: CommandType.StoredProcedure
            );

            return result;
        }

        public async Task<(IEnumerable<Route> Routes, int TotalRecords)> GetAllRoutesAsync(
            int pageNumber,
            int pageSize)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@PageNumber", pageNumber);
            parameters.Add("@PageSize", pageSize);

            var routes = (await _unitOfWork.Connection.QueryAsync<Route>(
                "sp_GetAllRoutes",
                parameters,
                commandType: CommandType.StoredProcedure
            )).ToList();

            var totalRecords = routes.FirstOrDefault()?.TotalRecords ?? 0;

            return (routes, totalRecords);
        }
    }

}
