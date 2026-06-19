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
    public class FlightDelayRepository : IFlightDelayRepository
    {
        private readonly IDapperUnitOfWork _unitOfWork;

        public FlightDelayRepository(IDapperUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Flight> ReportDelayAsync(
            int flightId,
            string delayType,
            int delayMinutes,
            string reason,
            int reportedBy)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@FlightId", flightId);
            parameters.Add("@DelayType", delayType);
            parameters.Add("@DelayMinutes", delayMinutes);
            parameters.Add("@Reason", reason);
            parameters.Add("@ReportedBy", reportedBy);

            return await _unitOfWork.Connection.QueryFirstOrDefaultAsync<Flight>(
                "sp_ReportFlightDelay",
                parameters,
                commandType: CommandType.StoredProcedure
            );
        }

        public async Task<IEnumerable<FlightDelayReason>> GetFlightDelayHistoryAsync(int flightId)
        {
            return await _unitOfWork.Connection.QueryAsync<FlightDelayReason>(
                "sp_GetFlightDelayHistory",
                new { FlightId = flightId },
                commandType: CommandType.StoredProcedure
            );
        }
    }
}
