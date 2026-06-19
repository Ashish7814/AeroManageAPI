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
    public class WeatherAlertRepository : IWeatherAlertRepository
    {
        private readonly IDapperUnitOfWork _unitOfWork;

        public WeatherAlertRepository(IDapperUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<WeatherAlert> CreateAlertAsync(WeatherAlert alert)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@AirportId", alert.AirportId);
            parameters.Add("@AlertType", alert.AlertType);
            parameters.Add("@Severity", alert.Severity);
            parameters.Add("@Description", alert.Description);
            parameters.Add("@StartTime", alert.StartTime);
            parameters.Add("@EndTime", alert.EndTime);

            return await _unitOfWork.Connection.QueryFirstOrDefaultAsync<WeatherAlert>(
                "sp_CreateWeatherAlert",
                parameters,
                commandType: CommandType.StoredProcedure
            );
        }

        public async Task<IEnumerable<WeatherAlert>> GetActiveAlertsAsync(int? airportId)
        {
            return await _unitOfWork.Connection.QueryAsync<WeatherAlert>(
                "sp_GetActiveWeatherAlerts",
                new { AirportId = airportId },
                commandType: CommandType.StoredProcedure
            );
        }

        public async Task<IEnumerable<Flight>> GetWeatherImpactedFlightsAsync(int alertId)
        {
            return await _unitOfWork.Connection.QueryAsync<Flight>(
                "sp_GetWeatherImpactedFlights",
                new { AlertId = alertId },
                commandType: CommandType.StoredProcedure
            );
        }

        public async Task<bool> DeactivateAlertAsync(int alertId)
        {
            var affected = await _unitOfWork.Connection.ExecuteAsync(
                "UPDATE WeatherAlerts SET IsActive = 0 WHERE AlertId = @AlertId",
                new { AlertId = alertId }
            );

            return affected > 0;
        }
    }
}
