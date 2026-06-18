using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AeroManage.Shared.Constant
{
    public class CacheKeys
    {
        public const string FlightPrefix = "flight:";
        public const string AirportPrefix = "airport:";
        public const string RoutePrefix = "route:";
        public const string AircraftPrefix = "aircraft:";
        public const string ScheduleTemplatePrefix = "schedule:";
        public const string WeatherAlertPrefix = "weather:";

        public static string Flight(int flightId) => $"{FlightPrefix}{flightId}";
        public static string FlightsByRoute(int routeId) => $"{FlightPrefix}route:{routeId}";
        public static string Airport(int airportId) => $"{AirportPrefix}{airportId}";
        public static string Route(int routeId) => $"{RoutePrefix}{routeId}";
        public static string Aircraft(int aircraftId) => $"{AircraftPrefix}{aircraftId}";
        public static string ScheduleTemplate(int templateId) => $"{ScheduleTemplatePrefix}{templateId}";
        public static string WeatherAlert(int airportId) => $"{WeatherAlertPrefix}{airportId}";
        public static string FlightDashboard(int? airportId, DateTime? date) =>
            $"dashboard:airport:{airportId}:date:{date?.ToString("yyyy-MM-dd") ?? "all"}";
    }
}
