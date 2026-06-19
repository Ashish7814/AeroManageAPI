using AeroManage.FlightManagement.Domain.Entities;
using AeroManage.FlightManagement.Domain.Interfaces;
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
    public class FlightNotificationRepository : IFlightNotificationRepository
    {
        private readonly IDapperUnitOfWork _unitOfWork;

        public FlightNotificationRepository(IDapperUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<FlightNotification> CreateNotificationAsync(
            int flightId,
            string notificationType,
            string message,
            string severity,
            int createdBy)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@FlightId", flightId);
            parameters.Add("@NotificationType", notificationType);
            parameters.Add("@Message", message);
            parameters.Add("@Severity", severity);
            parameters.Add("@CreatedBy", createdBy);

            return await _unitOfWork.Connection.QueryFirstOrDefaultAsync<FlightNotification>(
                "sp_CreateFlightNotification",
                parameters,
                commandType: CommandType.StoredProcedure
            );
        }

        public async Task<IEnumerable<FlightNotification>> GetFlightNotificationsAsync(int flightId, bool includeResolved)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@FlightId", flightId);
            parameters.Add("@IncludeResolved", includeResolved);

            return await _unitOfWork.Connection.QueryAsync<FlightNotification>(
                "sp_GetFlightNotifications",
                parameters,
                commandType: CommandType.StoredProcedure
            );
        }

        public async Task<FlightNotification> ResolveNotificationAsync(int notificationId, int resolvedBy)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@NotificationId", notificationId);
            parameters.Add("@ResolvedBy", resolvedBy);

            return await _unitOfWork.Connection.QueryFirstOrDefaultAsync<FlightNotification>(
                "sp_ResolveFlightNotification",
                parameters,
                commandType: CommandType.StoredProcedure
            );
        }
    }
}
