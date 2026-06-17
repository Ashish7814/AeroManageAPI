using AeroManage.Shared.DTos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AeroManage.FlightManagement.Domain.Interfaces
{
    public interface IFlightNumberRepository
    {
        Task<FlightNumberResultDto> GenerateFlightNumberAsync(string prefix);
    }
}
