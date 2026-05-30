using AeroManage.FlightManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AeroManage.FlightManagement.Infrastructure.Repositories.Interfaces
{
    public interface IFlightDelayRepository
    {
        Task<Flight> ReportDelayAsync(int flightId, string delayType, int delayMinutes, string reason, int reportedBy);
        Task<IEnumerable<FlightDelayReason>> GetFlightDelayHistoryAsync(int flightId);
    }
}
