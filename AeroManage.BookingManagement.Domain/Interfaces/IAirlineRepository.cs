using AeroManage.BookingManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AeroManage.BookingManagement.Domain.Interfaces
{
    public interface IAirlineRepository
    {
        Task<List<Airline>> GetActiveAirlinesAsync();
    }
}
