using AeroManage.FlightManagement.Domain.Entities;
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
    public class FlightRepository : IFlightRepository
    {
        private readonly string _connectionString;

        public FlightRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        private IDbConnection CreateConnection()
        {
            return new SqlConnection(_connectionString);
        }

        public async Task<int> CreateFlightAsync(Flight flight, int createdBy)
        {
            using var connection = CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add("@FlightNumber", flight.FlightNumber);
            parameters.Add("@RouteId", flight.RouteId);
            parameters.Add("@AircraftId", flight.AircraftId);
            parameters.Add("@DepartureDateTime", flight.DepartureDateTime);
            parameters.Add("@ArrivalDateTime", flight.ArrivalDateTime);
            parameters.Add("@FlightStatus", flight.FlightStatus);
            parameters.Add("@CreatedBy", flight.CreatedBy);

            var result = await connection.QueryFirstOrDefaultAsync<int>(
                "sp_CreateFlight",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return result;
        }
        public async Task<IEnumerable<Flight>> GetFlightsAsync()
        {
            using var connection = CreateConnection();

            //var result = await connection.QueryAsync<Flight>(
            //    "sp_GetFlights",
            //    commandType: CommandType.StoredProcedure
            //);

            //return result;

            using var multi = await connection.QueryMultipleAsync(
                "sp_GetFlights",
                commandType: CommandType.StoredProcedure);

            var flights = (await multi.ReadAsync<Flight>()).ToList();
            var prices = (await multi.ReadAsync<FlightPrice>()).ToList();
            var seats = (await multi.ReadAsync<FlightSeatAvailability>()).ToList();
            var gates = (await multi.ReadAsync<GateAssignment>()).ToList();

            foreach (var flight in flights)
            {
                flight.Prices = prices.Where(p => p.FlightId == flight.FlightId).ToList();
                flight.SeatAvailability = seats.Where(s => s.FlightId == flight.FlightId).ToList();
                flight.GateAssignments = gates.Where(g => g.FlightId == flight.FlightId).ToList();
            }

            return flights;
        }

        public async Task<Flight> GetFlightByIdAsync(int flightId)
        {
            using var connection = CreateConnection();

            var result = await connection.QueryFirstOrDefaultAsync<Flight>(
                "sp_GetFlightById",
                new { FlightId = flightId },
                commandType: CommandType.StoredProcedure
            );

            return result;
        }

        public async Task<(IEnumerable<Flight> Flights, int TotalRecords)> SearchFlightsAsync(
            int originAirportId,
            int destinationAirportId,
            DateTime departureDate,
            string flightNumber,
            int pageNumber,
            int pageSize)
        {
            using var connection = CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add("@OriginAirportId", originAirportId);
            parameters.Add("@DestinationAirportId", destinationAirportId);
            parameters.Add("@DepartureDate", departureDate);
            parameters.Add("@FlightNumber", flightNumber);
            parameters.Add("@PageNumber", pageNumber);
            parameters.Add("@PageSize", pageSize);

            var flights = (await connection.QueryAsync<Flight>(
                "sp_SearchFlights",
                parameters,
                commandType: CommandType.StoredProcedure
            )).ToList();

            var totalRecords = flights.FirstOrDefault()?.CreatedBy ?? 0;

            return (flights, totalRecords);
        }

        public async Task<Flight> UpdateFlightStatusAsync(int flightId, string newStatus, int changedBy)
        {
            using var connection = CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add("@FlightId", flightId);
            parameters.Add("@FlightStatus", newStatus);
            parameters.Add("@ChangedBy", changedBy);

            var result = await connection.QueryFirstOrDefaultAsync<Flight>(
                "sp_UpdateFlightStatus",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return result;
        }

        public async Task<FlightCrew> AssignFlightCrewAsync(int flightId, int userId, string crewRole)
        {
            using var connection = CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add("@FlightId", flightId);
            parameters.Add("@UserId", userId);
            parameters.Add("@CrewRole", crewRole);

            var result = await connection.QueryFirstOrDefaultAsync<FlightCrew>(
                "sp_AssignFlightCrew",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return result;
        }

        public async Task<IEnumerable<FlightCrew>> GetFlightCrewAsync(int flightId)
        {
            using var connection = CreateConnection();

            var result = await connection.QueryAsync<FlightCrew>(
                "sp_GetFlightCrew",
                new { FlightId = flightId },
                commandType: CommandType.StoredProcedure
            );

            return result;
        }

        public async Task<IEnumerable<FlightStatusHistory>> GetFlightStatusHistoryAsync(int flightId)
        {
            using var connection = CreateConnection();

            var result = await connection.QueryAsync<FlightStatusHistory>(
                "sp_GetFlightStatusHistory",
                new { FlightId = flightId },
                commandType: CommandType.StoredProcedure
            );

            return result;
        }

    }

}
