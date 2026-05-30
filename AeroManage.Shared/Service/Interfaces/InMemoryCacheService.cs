using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AeroManage.Shared.Service.Interfaces
{
    public class InMemoryCacheService : ICacheService
    {
        private readonly System.Collections.Concurrent.ConcurrentDictionary<string, (object Value, DateTime Expiration)> _cache;
        private readonly ILogger<InMemoryCacheService> _logger;

        public InMemoryCacheService(ILogger<InMemoryCacheService> logger)
        {
            _cache = new System.Collections.Concurrent.ConcurrentDictionary<string, (object, DateTime)>();
            _logger = logger;
        }

        public Task<T> GetAsync<T>(string key)
        {
            try
            {
                if (_cache.TryGetValue(key, out var cached))
                {
                    if (cached.Expiration > DateTime.UtcNow)
                    {
                        return Task.FromResult((T)cached.Value);
                    }

                    _cache.TryRemove(key, out _);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting cache key: {key}");
            }

            return Task.FromResult(default(T));
        }

        public Task SetAsync<T>(string key, T value, TimeSpan? expiration = null)
        {
            try
            {
                var exp = DateTime.UtcNow.Add(expiration ?? TimeSpan.FromHours(1));
                _cache.AddOrUpdate(key, (value, exp), (k, old) => (value, exp));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error setting cache key: {key}");
            }

            return Task.CompletedTask;
        }

        public Task RemoveAsync(string key)
        {
            try
            {
                _cache.TryRemove(key, out _);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error removing cache key: {key}");
            }

            return Task.CompletedTask;
        }

        public Task<bool> ExistsAsync(string key)
        {
            return Task.FromResult(_cache.ContainsKey(key));
        }

        public Task<bool> SetAddAsync(string key, string value)
        {
            throw new NotImplementedException();
        }

        public Task<bool> SetRemoveAsync(string key, string value)
        {
            throw new NotImplementedException();
        }

        public Task<long> IncrementAsync(string key, long value = 1)
        {
            throw new NotImplementedException();
        }

        public Task<long> DecrementAsync(string key, long value = 1)
        {
            throw new NotImplementedException();
        }

        public Task<bool> SetExpirationAsync(string key, TimeSpan expiration)
        {
            throw new NotImplementedException();
        }

        public Task<TimeSpan?> GetTimeToLiveAsync(string key)
        {
            throw new NotImplementedException();
        }
    }
}
