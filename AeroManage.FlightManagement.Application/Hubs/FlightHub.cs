using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AeroManage.FlightManagement.Application.Hubs
{
    [Authorize]
    public class FlightHub : Hub
    {
        private readonly ILogger<FlightHub> _logger;

        public FlightHub(ILogger<FlightHub> logger)
        {
            _logger = logger;
        }

        public override async Task OnConnectedAsync()
        {
            var userId = Context.UserIdentifier;
            _logger.LogInformation($"User {userId} connected to FlightHub");

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var userId = Context.UserIdentifier;
            _logger.LogInformation($"User {userId} disconnected from FlightHub");

            await base.OnDisconnectedAsync(exception);
        }

        /// <summary>
        /// Subscribe to specific flight updates
        /// </summary>
        public async Task SubscribeToFlight(int flightId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"Flight_{flightId}");
            _logger.LogInformation($"User {Context.UserIdentifier} subscribed to Flight_{flightId}");
        }

        /// <summary>
        /// Unsubscribe from specific flight updates
        /// </summary>
        public async Task UnsubscribeFromFlight(int flightId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"Flight_{flightId}");
            _logger.LogInformation($"User {Context.UserIdentifier} unsubscribed from Flight_{flightId}");
        }

        /// <summary>
        /// Subscribe to airport updates
        /// </summary>
        public async Task SubscribeToAirport(int airportId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"Airport_{airportId}");
            _logger.LogInformation($"User {Context.UserIdentifier} subscribed to Airport_{airportId}");
        }

        /// <summary>
        /// Unsubscribe from airport updates
        /// </summary>
        public async Task UnsubscribeFromAirport(int airportId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"Airport_{airportId}");
            _logger.LogInformation($"User {Context.UserIdentifier} unsubscribed from Airport_{airportId}");
        }

        /// <summary>
        /// Subscribe to all flights (admin/staff)
        /// </summary>
        [Authorize(Roles = "Admin,AirlineStaff")]
        public async Task SubscribeToAllFlights()
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, "AllFlights");
            _logger.LogInformation($"Admin user {Context.UserIdentifier} subscribed to AllFlights");
        }

        /// <summary>
        /// Request current flight status
        /// </summary>
        public async Task RequestFlightStatus(int flightId)
        {
            // This would trigger a query to get current status
            // and send it back to the requesting client
            await Clients.Caller.SendAsync("FlightStatusRequested", flightId);
        }
    }
}
