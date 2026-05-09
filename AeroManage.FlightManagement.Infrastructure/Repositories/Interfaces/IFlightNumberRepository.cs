using AeroManage.Shared.DTos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AeroManage.FlightManagement.Infrastructure.Repositories.Interfaces
{
    public interface IFlightNumberRepository
    {
        Task<FlightNumberResultDto> GenerateFlightNumberAsync(string prefix);
    }
}
