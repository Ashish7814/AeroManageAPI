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
    public class FlightDashboardRepository : IFlightDashboardRepository
    {
        private readonly IDapperUnitOfWork _unitOfWork;

        public FlightDashboardRepository(IDapperUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<FlightDashboard>> GetFlightDashboardAsync(
            int? airportId,
            DateTime? date,
            string status)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@AirportId", airportId);
            parameters.Add("@Date", date);
            parameters.Add("@Status", status);

            return await _unitOfWork.Connection.QueryAsync<FlightDashboard>(
                "sp_GetFlightDashboard",
                parameters,
                commandType: CommandType.StoredProcedure
            );
        }

        public async Task<Flight> UpdateBoardingStatusAsync(int flightId, string boardingStatus, int updatedBy)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@FlightId", flightId);
            parameters.Add("@BoardingStatus", boardingStatus);
            parameters.Add("@UpdatedBy", updatedBy);

            return await _unitOfWork.Connection.QueryFirstOrDefaultAsync<Flight>(
                "sp_UpdateBoardingStatus",
                parameters,
                commandType: CommandType.StoredProcedure
            );
        }
    }
}
