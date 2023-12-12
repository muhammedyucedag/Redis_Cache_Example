using Distributed.Caching.Models;
using System.Text;
using System.Xml.Serialization;

namespace Distributed.Caching.Service
{
    public class CurrencyService : ICurrencyService
    {
        private readonly HttpClient _client;

        public CurrencyService()
        {
            _client = new HttpClient();
        }

        public async Task<CurrencyResponse> GetCurrencyDataAsync()
        {
            var response = await _client.GetStringAsync("https://www.tcmb.gov.tr/kurlar/today.xml");

            XmlSerializer xmlSerializer = new XmlSerializer(typeof(CurrencyResponse));
            using (MemoryStream memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(response)))
            {
                return (CurrencyResponse)xmlSerializer.Deserialize(memoryStream);
            }
        }
    }
}
