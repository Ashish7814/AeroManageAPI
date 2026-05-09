using AeroManage.FlightManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AeroManage.FlightManagement.Infrastructure.Repositories.Interfaces
{
    public interface IAirportRepository
    {
        Task<Airport> CreateAirportAsync(Airport airport);
        Task<Airport> GetAirportByIdAsync(int airportId);
        Task<(IEnumerable<Airport> Airports, int TotalRecords)> GetAllAirportsAsync(int pageNumber, int pageSize);
    }
}
