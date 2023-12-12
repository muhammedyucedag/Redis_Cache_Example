using Distributed.Caching.Models;
using Distributed.Caching.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using System.Xml.Linq;

namespace Distributed.Caching.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CurrencyController : ControllerBase
    {
        private readonly ICurrencyService _currencyService;
        private readonly IDistributedCache _distributedCache;

        public CurrencyController(ICurrencyService currencyService,IDistributedCache distributedCache)
        {
            _currencyService = currencyService;
            _distributedCache = distributedCache;
        }

        [HttpGet]
        public async Task<IActionResult> GetCurrencyData()
        {
            // Önce önbellekte veriyi kontrol et
            var cachedData = await _distributedCache.GetStringAsync("CurrencyData");

            if (cachedData != null)
            {
                // Eğer önbellekte varsa, önbellekten al ve döndür
                var cachedCurrencies = JsonConvert.DeserializeObject<List<Currency>>(cachedData);
                return Ok(cachedCurrencies);
            }

            // Eğer önbellekte yoksa, veriyi al, önbelleğe ekle ve döndür
            var currencyResponse = await _currencyService.GetCurrencyDataAsync();

            var selectedCurrencies = currencyResponse.Currency
                .Where(x => x.CurrencyCode == "USD" || x.CurrencyCode == "EUR")
                 .Select(x => new
                 {
                     x.Isim,
                     x.CurrencyCode,
                     x.ForexBuying,
                     x.ForexSelling,
                 })
                 .ToList();

            // Veriyi JSON formatına çevir
            var serializedData = JsonConvert.SerializeObject(selectedCurrencies);

            // JSON string'i Redis üzerinde sakla
            await _distributedCache.SetStringAsync("CurrencyData", serializedData, new DistributedCacheEntryOptions
            {
                AbsoluteExpiration = DateTime.Now.AddMinutes(15),
                SlidingExpiration = TimeSpan.FromSeconds(10)
            });

            return Ok(selectedCurrencies);
        }

    }
}
