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
    public class AirportRepository : IAirportRepository
    {
        private readonly IDapperUnitOfWork _unitOfWork;

        public AirportRepository(IDapperUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Airport> CreateAirportAsync(Airport airport)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@AirportCode", airport.AirportCode);
            parameters.Add("@ICAOCode", airport.ICAOCode);
            parameters.Add("@AirportName", airport.AirportName);
            parameters.Add("@City", airport.City);
            parameters.Add("@State", airport.State);
            parameters.Add("@Country", airport.Country);
            parameters.Add("@Latitude", airport.Latitude);
            parameters.Add("@Longitude", airport.Longitude);
            parameters.Add("@Timezone", airport.Timezone);

            var result = await _unitOfWork.Connection.QueryFirstOrDefaultAsync<Airport>(
                "sp_CreateAirport",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return result;
        }

        public async Task<Airport> GetAirportByIdAsync(int airportId)
        {
            var result = await _unitOfWork.Connection.QueryFirstOrDefaultAsync<Airport>(
                "sp_GetAirportById",
                new { AirportId = airportId },
                commandType: CommandType.StoredProcedure
            );

            return result;
        }

        public async Task<(IEnumerable<Airport> Airports, int TotalRecords)> GetAllAirportsAsync(
            int pageNumber,
            int pageSize)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@PageNumber", pageNumber);
            parameters.Add("@PageSize", pageSize);

            var airports = (await _unitOfWork.Connection.QueryAsync<Airport>(
                "GetAirports",
                parameters,
                commandType: CommandType.StoredProcedure
            )).ToList();

            var totalRecords = airports.FirstOrDefault()?.TotalRecords ?? 0;

            return (airports, totalRecords);
        }

    }
}
