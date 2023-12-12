using Distributed.Caching.Models;

namespace Distributed.Caching.Service
{
    public interface ICurrencyService
    {
        Task<CurrencyResponse> GetCurrencyDataAsync();
    }
}
