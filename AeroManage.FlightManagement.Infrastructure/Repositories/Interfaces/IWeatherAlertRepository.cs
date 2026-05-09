using AeroManage.FlightManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AeroManage.FlightManagement.Infrastructure.Repositories.Interfaces
{
    public interface IWeatherAlertRepository
    {
        Task<WeatherAlert> CreateAlertAsync(WeatherAlert alert);
        Task<IEnumerable<WeatherAlert>> GetActiveAlertsAsync(int? airportId);
        Task<IEnumerable<Flight>> GetWeatherImpactedFlightsAsync(int alertId);
        Task<bool> DeactivateAlertAsync(int alertId);
    }
}
