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
    public class RouteLayoverRepository : IRouteLayoverRepository
    {
        private readonly IDapperUnitOfWork _unitOfWork;

        public RouteLayoverRepository(IDapperUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<RouteLayover> AddLayoverAsync(int routeId, int airportId, int layoverSequence, int minMinutes, int maxMinutes)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@RouteId", routeId);
            parameters.Add("@AirportId", airportId);
            parameters.Add("@LayoverSequence", layoverSequence);
            parameters.Add("@MinimumLayoverMinutes", minMinutes);
            parameters.Add("@MaximumLayoverMinutes", maxMinutes);

            return await _unitOfWork.Connection.QueryFirstOrDefaultAsync<RouteLayover>(
                "sp_AddRouteLayover",
                parameters,
                commandType: CommandType.StoredProcedure
            );
        }

        public async Task<IEnumerable<RouteLayover>> GetRouteLayoversAsync(int routeId)
        {
            // Get route info and layovers
            using var multi = await _unitOfWork.Connection.QueryMultipleAsync(
                "sp_GetRouteWithLayovers",
                new { RouteId = routeId },
                commandType: CommandType.StoredProcedure
            );

            // Skip route details (first result set)
            await multi.ReadAsync();

            // Get layovers (second result set)
            return await multi.ReadAsync<RouteLayover>();
        }

        public async Task<bool> DeleteLayoverAsync(int layoverId)
        {
            var affected = await _unitOfWork.Connection.ExecuteAsync(
                "UPDATE RouteLayovers SET IsActive = 0 WHERE LayoverId = @LayoverId",
                new { LayoverId = layoverId }
            );

            return affected > 0;
        }
    }

}
