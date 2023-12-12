using Distributed.Caching.Models;
using Distributed.Caching.Service;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;

namespace Distributed.Caching.Hangfire
{
    public class CurrencyJob
    {
        private readonly ICurrencyIntegrationService _currencyService;
        private readonly IDistributedCache _distributedCache;

        public CurrencyJob(ICurrencyIntegrationService currencyService, IDistributedCache distributedCache)
        {
            _currencyService = currencyService;
            _distributedCache = distributedCache;
        }

        public async Task UpdateCurrencyData()
        {
            var currencyResponse = await _currencyService.GetCurrencyDataAsync();

            var selectedCurrencies = currencyResponse.Currency
                .FirstOrDefault(x => x.CurrencyCode == "USD");

            var serializedData = JsonConvert.SerializeObject(selectedCurrencies);

            await _distributedCache.SetStringAsync("USDCurrency", serializedData, new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(6)
            });
        }
    }
}
