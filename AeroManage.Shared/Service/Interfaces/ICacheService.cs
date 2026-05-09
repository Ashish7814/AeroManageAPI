using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AeroManage.Shared.Service.Interfaces
{
    public interface ICacheService
    {
        Task<T> GetAsync<T>(string key);
        Task SetAsync<T>(string key, T value, TimeSpan? expiration = null);
        Task RemoveAsync(string key);
        Task<bool> ExistsAsync(string key);
        Task<bool> SetAddAsync(string key, string value);
        Task<bool> SetRemoveAsync(string key, string value);
        Task<long> IncrementAsync(string key, long value = 1);
        Task<long> DecrementAsync(string key, long value = 1);
        Task<bool> SetExpirationAsync(string key, TimeSpan expiration);
        Task<TimeSpan?> GetTimeToLiveAsync(string key);
    }
}
