using AeroManage.Shared.Service.Interfaces;
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
    public class CacheService : ICacheService
    {
        private readonly IConnectionMultiplexer _redis;
        private readonly IDatabase _database;
        private readonly ILogger<CacheService> _logger;

        public CacheService(
            IConnectionMultiplexer redis,
            ILogger<CacheService> logger)
        {
            _redis = redis ?? throw new ArgumentNullException(nameof(redis));
            _database = _redis.GetDatabase();
            _logger = logger;
        }

        public async Task<T> GetAsync<T>(string key)
        {
            try
            {
                var value = await _database.StringGetAsync(key);

                if (value.IsNullOrEmpty)
                {
                    return default;
                }

                return JsonSerializer.Deserialize<T>(value);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting cache key: {key}");
                return default;
            }
        }

        public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null)
        {
            try
            {
                var serialized = JsonSerializer.Serialize(value);

                await _database.StringSetAsync(
                    key,
                    serialized,
                    expiration ?? TimeSpan.FromHours(1)
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error setting cache key: {key}");
            }
        }

        public async Task RemoveAsync(string key)
        {
            try
            {
                await _database.KeyDeleteAsync(key);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error removing cache key: {key}");
            }
        }

        public async Task<bool> ExistsAsync(string key)
        {
            try
            {
                return await _database.KeyExistsAsync(key);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error checking cache key existence: {key}");
                return false;
            }
        }

        public async Task<bool> SetAddAsync(string key, string value)
        {
            try
            {
                return await _database.SetAddAsync(key, value);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error adding to set: {key}");
                return false;
            }
        }

        public async Task<bool> SetRemoveAsync(string key, string value)
        {
            try
            {
                return await _database.SetRemoveAsync(key, value);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error removing from set: {key}");
                return false;
            }
        }

        public async Task<long> IncrementAsync(string key, long value = 1)
        {
            try
            {
                return await _database.StringIncrementAsync(key, value);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error incrementing key: {key}");
                return 0;
            }
        }

        public async Task<long> DecrementAsync(string key, long value = 1)
        {
            try
            {
                return await _database.StringDecrementAsync(key, value);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error decrementing key: {key}");
                return 0;
            }
        }

        public async Task<bool> SetExpirationAsync(string key, TimeSpan expiration)
        {
            try
            {
                return await _database.KeyExpireAsync(key, expiration);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error setting expiration for key: {key}");
                return false;
            }
        }

        public async Task<TimeSpan?> GetTimeToLiveAsync(string key)
        {
            try
            {
                return await _database.KeyTimeToLiveAsync(key);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting TTL for key: {key}");
                return null;
            }
        }
    }
}
