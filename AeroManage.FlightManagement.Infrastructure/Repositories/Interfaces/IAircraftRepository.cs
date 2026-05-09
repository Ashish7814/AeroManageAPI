using AeroManage.FlightManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AeroManage.FlightManagement.Infrastructure.Repositories.Interfaces
{
    public interface IAircraftRepository
    {
        Task<Aircraft> CreateAircraftAsync(Aircraft aircraft);
        Task<Aircraft> GetAircraftByIdAsync(int? aircraftId);
        Task<(IEnumerable<Aircraft> Aircraft, int TotalRecords)> GetAllAircraftAsync(int pageNumber, int pageSize, string status);
        Task<Aircraft> UpdateAircraftStatusAsync(int aircraftId, string status);
    }
}
