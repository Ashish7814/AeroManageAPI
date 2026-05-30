using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AeroManage.BookingManagement.Application.Hubs
{
    [Authorize]
    public class BookingHub : Hub
    {
        private readonly ILogger<BookingHub> _logger;

        public BookingHub(ILogger<BookingHub> logger)
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
        /// Subscribe to specific booking updates
        /// </summary>
        public async Task SubscribeToBooking(int bookingId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"Booking_{bookingId}");
            _logger.LogInformation($"User {Context.UserIdentifier} subscribed to Booking_{bookingId}");
        }

        /// <summary>
        /// Unsubscribe from booking updates
        /// </summary>
        public async Task UnsubscribeFromBooking(int bookingId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"Booking_{bookingId}");
            _logger.LogInformation($"User {Context.UserIdentifier} unsubscribed from Booking_{bookingId}");
        }

        /// <summary>
        /// Subscribe to user's bookings
        /// </summary>
        public async Task SubscribeToUserBookings(int userId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"User_{userId}_Bookings");
            _logger.LogInformation($"User {Context.UserIdentifier} subscribed to User_{userId}_Bookings");
        }

        /// <summary>
        /// Request booking status update
        /// </summary>
        public async Task RequestBookingStatus(int bookingId)
        {
            await Clients.Caller.SendAsync("BookingStatusRequested", bookingId);
        }

        // ==================== SERVER-TO-CLIENT EVENTS ====================
        // These methods are called from command handlers to broadcast updates

        // BookingCreated(bookingId, bookingReference)
        // BookingConfirmed(bookingId, bookingReference, pnr)
        // PaymentProcessing(bookingId)
        // PaymentSucceeded(bookingId, amount)
        // PaymentFailed(bookingId, reason)
        // SeatReserved(bookingId, seatNumber)
        // SeatReleased(bookingId, seatNumber)
        // BookingCancelled(bookingId, refundAmount)
        // ETicketGenerated(bookingId, downloadUrl)
        // BookingModified(bookingId, changes)
    }

}

