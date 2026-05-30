using AeroManage.FlightManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AeroManage.FlightManagement.Infrastructure.Repositories.Interfaces
{
    public interface IFlightRepository
    {
        Task<int> CreateFlightAsync(Flight flight, int createdBy);
        Task<Flight> GetFlightByIdAsync(int flightId);
        Task<IEnumerable<Flight>> GetFlightsAsync();
        Task<(IEnumerable<Flight> Flights, int TotalRecords)> SearchFlightsAsync(
            int originAirportId,
            int destinationAirportId,
            DateTime departureDate,
            string flightNumber,
            int pageNumber,
            int pageSize);
        Task<Flight> UpdateFlightStatusAsync(int flightId, string newStatus, int changedBy);
        Task<FlightCrew> AssignFlightCrewAsync(int flightId, int userId, string crewRole);
        Task<IEnumerable<FlightCrew>> GetFlightCrewAsync(int flightId);
        Task<IEnumerable<FlightStatusHistory>> GetFlightStatusHistoryAsync(int flightId);
    }
}
