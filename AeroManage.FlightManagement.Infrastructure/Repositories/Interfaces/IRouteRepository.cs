using AeroManage.FlightManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AeroManage.FlightManagement.Infrastructure.Repositories.Interfaces
{
    public interface IRouteRepository
    {
        Task<Route> CreateRouteAsync(Route route);
        Task<Route> GetRouteByIdAsync(int routeId);
        Task<(IEnumerable<Route> Routes, int TotalRecords)> GetAllRoutesAsync(int pageNumber, int pageSize);
    }
}
