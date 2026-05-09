using AeroManage.FlightManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AeroManage.FlightManagement.Infrastructure.Repositories.Interfaces
{
    public interface IFlightDashboardRepository
    {
        Task<IEnumerable<FlightDashboard>> GetFlightDashboardAsync(int? airportId, DateTime? date, string status);
        Task<Flight> UpdateBoardingStatusAsync(int flightId, string boardingStatus, int updatedBy);
    }
}
