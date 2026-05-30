using AeroManage.FlightManagement.Domain.Entities;
using AeroManage.FlightManagement.Infrastructure.Repositories.Interfaces;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AeroManage.FlightManagement.Infrastructure.Repositories.Implementation
{
    public class FlightScheduleTemplateRepository : IFlightScheduleTemplateRepository
    {
        private readonly string _connectionString;

        public FlightScheduleTemplateRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        private IDbConnection CreateConnection() => new SqlConnection(_connectionString);

        public async Task<FlightScheduleTemplate> CreateTemplateAsync(FlightScheduleTemplate template, int createdBy)
        {
            using var connection = CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add("@TemplateName", template.TemplateName);
            parameters.Add("@FlightNumberPrefix", template.FlightNumberPrefix);
            parameters.Add("@RouteId", template.RouteId);
            parameters.Add("@AircraftId", template.AircraftId);
            //parameters.Add("@RecurrenceType", template.RecurrenceType);
            //parameters.Add("@DaysOfWeek", template.DaysOfWeek);
            //parameters.Add("@DayOfMonth", template.DayOfMonth);
            parameters.Add("@StartDate", template.StartDate);
            parameters.Add("@EndDate", template.EndDate);
            parameters.Add("@DepartureTime", template.DepartureTime);
            parameters.Add("@ArrivalTime", template.ArrivalTime);
            //parameters.Add("@EconomyPrice", template.EconomyPrice);
            //parameters.Add("@BusinessPrice", template.BusinessPrice);
            //parameters.Add("@FirstClassPrice", template.FirstClassPrice);
            parameters.Add("@CreatedBy", createdBy);

            return await connection.QueryFirstOrDefaultAsync<FlightScheduleTemplate>(
                "sp_CreateFlightScheduleTemplate",
                parameters,
                commandType: CommandType.StoredProcedure
            );
        }

        public async Task<FlightScheduleTemplate> GetTemplateByIdAsync(int templateId)
        {
            using var connection = CreateConnection();

            return await connection.QueryFirstOrDefaultAsync<FlightScheduleTemplate>(
                "SELECT * FROM FlightScheduleTemplates WHERE TemplateId = @TemplateId",
                new { TemplateId = templateId }
            );
        }

        public async Task<(IEnumerable<FlightScheduleTemplate> Templates, int TotalRecords)> GetAllTemplatesAsync(
            int pageNumber,
            int pageSize,
            bool? isActive)
        {
            using var connection = CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add("@PageNumber", pageNumber);
            parameters.Add("@PageSize", pageSize);
            parameters.Add("@IsActive", isActive);

            var templates = (await connection.QueryAsync<FlightScheduleTemplate>(
                "sp_GetAllScheduleTemplates",
                parameters,
                commandType: CommandType.StoredProcedure
            )).ToList();

            var totalRecords = templates.FirstOrDefault()?.TotalRecords ?? 0;

            return (templates, totalRecords);
        }

        public async Task<int> GenerateFlightsFromTemplateAsync(int templateId, DateTime fromDate, DateTime toDate, int createdBy)
        {
            using var connection = CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add("@TemplateId", templateId);
            parameters.Add("@GenerateFromDate", fromDate);
            parameters.Add("@GenerateToDate", toDate);
            parameters.Add("@CreatedBy", createdBy);

            var result = await connection.QueryFirstOrDefaultAsync<dynamic>(
                "sp_GenerateFlightsFromTemplate",
                parameters,
                commandType: CommandType.StoredProcedure,
                commandTimeout: 120 // Longer timeout for bulk generation
            );

            return result?.FlightsCreated ?? 0;
        }

        public async Task<bool> DeactivateTemplateAsync(int templateId)
        {
            using var connection = CreateConnection();

            var affected = await connection.ExecuteAsync(
                "UPDATE FlightScheduleTemplates SET IsActive = 0 WHERE TemplateId = @TemplateId",
                new { TemplateId = templateId }
            );

            return affected > 0;
        }
    }
}
