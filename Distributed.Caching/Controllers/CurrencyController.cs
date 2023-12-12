using Distributed.Caching.Service;
using Microsoft.AspNetCore.Mvc;

namespace Distributed.Caching.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CurrencyController : ControllerBase
    {
        private readonly ICurrencyService _currencyService;

        public CurrencyController(ICurrencyService currencyService)
        {
            _currencyService = currencyService;
        }

        [HttpGet]
        public async Task<IActionResult> GetUSDCurrency()
        { 
            var currencies = await _currencyService.GetUSDCurrencyAsync();
            return Ok(currencies);
        }

    }
}
