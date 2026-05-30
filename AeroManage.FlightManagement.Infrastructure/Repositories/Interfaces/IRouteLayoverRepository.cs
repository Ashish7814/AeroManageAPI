using AeroManage.FlightManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AeroManage.FlightManagement.Infrastructure.Repositories.Interfaces
{
    public interface IRouteLayoverRepository
    {
        Task<RouteLayover> AddLayoverAsync(int routeId, int airportId, int layoverSequence, int minMinutes, int maxMinutes);
        Task<IEnumerable<RouteLayover>> GetRouteLayoversAsync(int routeId);
        Task<bool> DeleteLayoverAsync(int layoverId);
    }
}
