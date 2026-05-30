using AeroManage.FlightManagement.Application.Services.Interfaces;
using AeroManage.Shared.Service.Interfaces;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace AeroManage.Shared.Service.Implementation
{
    public class RedisCacheService
    {
        private readonly IDistributedCache _cache;
        private readonly IConnectionMultiplexer _redis;
        private readonly ILogger<RedisCacheService> _logger;

        public RedisCacheService(
            IDistributedCache cache,
            IConnectionMultiplexer redis,
            ILogger<RedisCacheService> logger)
        {
            _cache = cache;
            _redis = redis;
            _logger = logger;
        }

        public async Task<T> GetAsync<T>(string key) where T : class
        {
            try
            {
                var cachedData = await _cache.GetStringAsync(key);

                if (string.IsNullOrEmpty(cachedData))
                {
                    return null;
                }

                return JsonSerializer.Deserialize<T>(cachedData);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving cached data for key: {key}");
                return null;
            }
        }

        public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null) where T : class
        {
            try
            {
                var serializedData = JsonSerializer.Serialize(value);

                var options = new DistributedCacheEntryOptions();

                if (expiration.HasValue)
                {
                    options.AbsoluteExpirationRelativeToNow = expiration.Value;
                }
                else
                {
                    // Default expiration: 30 minutes
                    options.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30);
                }

                await _cache.SetStringAsync(key, serializedData, options);

                _logger.LogInformation($"Cached data for key: {key}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error caching data for key: {key}");
            }
        }

        public async Task RemoveAsync(string key)
        {
            try
            {
                await _cache.RemoveAsync(key);
                _logger.LogInformation($"Removed cached data for key: {key}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error removing cached data for key: {key}");
            }
        }

        public async Task<bool> ExistsAsync(string key)
        {
            try
            {
                var data = await _cache.GetStringAsync(key);
                return !string.IsNullOrEmpty(data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error checking if key exists: {key}");
                return false;
            }
        }

        public async Task RemoveByPatternAsync(string pattern)
        {
            try
            {
                var server = _redis.GetServer(_redis.GetEndPoints()[0]);
                var keys = server.Keys(pattern: pattern);

                foreach (var key in keys)
                {
                    await _cache.RemoveAsync(key.ToString());
                }

                _logger.LogInformation($"Removed all cached data matching pattern: {pattern}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error removing cached data by pattern: {pattern}");
            }
        }
    }

    // Cache key constants
    public static class CacheKeys
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
