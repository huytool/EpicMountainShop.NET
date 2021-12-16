﻿using ASC.Business.Interface;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Webb.Data
{
    public class MasterDataCacheOperations:IMasterDataCacheOperations
    {
        private readonly IDistributedCache _cache;
        private readonly IMasterDataOperations _masterData;
        private readonly string MasterDataCacheName = "MasterDataCache";
        public MasterDataCacheOperations(IDistributedCache cache, IMasterDataOperations
       masterData)
        {
            _cache = cache;
            _masterData = masterData;
        }
        public async Task CreateMasterDataCacheAsync()
        {
            var masterDataCache = new MasterDataCache
            {
                Keys = (await _masterData.GetAllMasterKeysAsync()).Where(p => p.IsActive == true).ToList(),
                Values = (await _masterData.GetAllMasterValuesAsync()).Where(p => p.IsActive == true).ToList()
            };
            await _cache.SetStringAsync(MasterDataCacheName, JsonConvert.SerializeObject(masterDataCache));
        }
        public async Task<MasterDataCache> GetMasterDataCacheAsync()
        {
            return JsonConvert.DeserializeObject<MasterDataCache>(await _cache.GetStringAsync(MasterDataCacheName));
        }
    }
}
