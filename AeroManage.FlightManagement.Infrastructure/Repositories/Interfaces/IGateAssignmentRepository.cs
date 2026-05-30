using AeroManage.FlightManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AeroManage.FlightManagement.Infrastructure.Repositories.Interfaces
{
    public interface IGateAssignmentRepository
    {
        Task<GateAssignment> AssignGateAsync(int flightId, string gateNumber, string gateType, DateTime scheduledTime, int assignedBy);
        Task<Flight> ChangeFlightGateAsync(int flightId, string newGateNumber, string gateType, string reason, int changedBy);
        Task<IEnumerable<GateAssignment>> GetFlightGateAssignmentsAsync(int flightId);
    }
}
