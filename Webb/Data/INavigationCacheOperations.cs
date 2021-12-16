using ASC.Utilities.Navigation;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Webb.Data
{
    public interface INavigationCacheOperations
    {
        Task<NavigationMenu> GetNavigationCacheAsync();
        Task CreateNavigationCacheAsync();
    }
    public class NavigationCacheOperations : INavigationCacheOperations
    {
        private readonly IDistributedCache _cache;
        private readonly string NavigationCacheName = "NavigationCache";
        public NavigationCacheOperations(IDistributedCache cache)
        {
            _cache = cache;
        }
        public async Task CreateNavigationCacheAsync()
        {
            await _cache.SetStringAsync(NavigationCacheName, File.ReadAllText("Navigation/Navigation.json"));
        }
        public async Task<NavigationMenu> GetNavigationCacheAsync()
        {
            return JsonConvert.DeserializeObject<NavigationMenu>(await _cache.GetStringAsync(NavigationCacheName));
        }
    }
}
