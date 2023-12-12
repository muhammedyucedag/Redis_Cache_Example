using AsyncLazy;
using Distributed.Caching.Models;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;

namespace Distributed.Caching.Service
{
    public class CurrencyService : ICurrencyService
    {
        private readonly IDistributedCache _distributedCache;
        private readonly AsyncLazy<Currency> _USDCurrency;

        public CurrencyService(IDistributedCache distributedCache)
        {
            _distributedCache = distributedCache;
            _USDCurrency = new AsyncLazy<Currency>( async () => await _GetUSDCurrencyAsync());
        }

        private async Task<Currency> _GetUSDCurrencyAsync()
        {
            var cachedData = await _distributedCache.GetStringAsync("USDCurrency");

            if (cachedData != null)
            {
                var cachedCurrencies = JsonConvert.DeserializeObject<Currency>(cachedData);
                return cachedCurrencies;
            }
            return new();
        }

        public async Task<Currency> GetUSDCurrencyAsync()
        {
            return await _USDCurrency.GetValueAsync();
        }
    }
}
