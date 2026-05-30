using AeroManage.FlightManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AeroManage.FlightManagement.Infrastructure.Repositories.Interfaces
{
    public interface IFlightScheduleTemplateRepository
    {
        Task<FlightScheduleTemplate> CreateTemplateAsync(FlightScheduleTemplate template, int createdBy);
        Task<FlightScheduleTemplate> GetTemplateByIdAsync(int templateId);
        Task<(IEnumerable<FlightScheduleTemplate> Templates, int TotalRecords)> GetAllTemplatesAsync(int pageNumber, int pageSize, bool? isActive);
        Task<int> GenerateFlightsFromTemplateAsync(int templateId, DateTime fromDate, DateTime toDate, int createdBy);
        Task<bool> DeactivateTemplateAsync(int templateId);
    }
}
