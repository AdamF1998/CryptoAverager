using Binance.Net;
using Binance.Net.Objects;
using CryptoExchange.Net.Authentication;
using System.Net;
using System.Security;
using System.Threading.Tasks;

namespace CryptoAverager.Service
{
    public class BinanceService : IBinanceService
    {
        public async Task CreateClient(string key, string secret)
        {
            SecureString secureKeyString = new NetworkCredential("Key", key).SecurePassword;
            SecureString secureSecretString = new NetworkCredential("Secret", secret).SecurePassword;

            var client = new BinanceClient(new BinanceClientOptions() {ApiCredentials = new ApiCredentials(secureKeyString, secureSecretString) });

            var response = await client.Spot.Market.GetTradeHistoryAsync("VET");

            if (!response.Success)
            {
                var error = response.Error;
            }

            var result = response.Data;
        }
    }
}
