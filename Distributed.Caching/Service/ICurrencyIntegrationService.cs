using Distributed.Caching.Models;

namespace Distributed.Caching.Service
{
    public interface ICurrencyIntegrationService
    {
        Task<CurrencyResponse> GetCurrencyDataAsync();
    }
}
