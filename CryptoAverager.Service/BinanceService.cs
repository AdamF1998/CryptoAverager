using Binance.Net;
using Binance.Net.Objects;
using CryptoExchange.Net.Authentication;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Net;
using System.Security;
using System.Threading.Tasks;

namespace CryptoAverager.Service
{
    public class BinanceService : IBinanceService
    {
        private readonly IMemoryCache _cache;

        public BinanceService(IMemoryCache cache)
        {
            _cache = cache;
        }

        public void CreateClient(string key, string secret)
        {
            SecureString secureKeyString = new NetworkCredential("Key", key).SecurePassword;
            SecureString secureSecretString = new NetworkCredential("Secret", secret).SecurePassword;

            BinanceClient entry;

            // Store the Binance Client inside the memory cache - I don't want to be storing the ApiKey and the Secret
            if(!_cache.TryGetValue(key, out entry))
            {
                entry = new BinanceClient(new BinanceClientOptions() { ApiCredentials = new ApiCredentials(secureKeyString, secureSecretString) });

                // Give the user 60 minutes before having to reauthenticate with the api again
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromMinutes(60));

                _cache.Set(key, entry, cacheEntryOptions);
            }
        }

        public void CalculateCoinAverages()
        {

        }

        public async Task<List<dynamic>> GetAllCoinsWithABalance(BinanceClient client)
        {
            List<dynamic> userHeldCoins = new List<dynamic>();

            var response = await client.General.GetAccountInfoAsync();

            if (!response.Success)
            {
                //ToDo: I need to handle this...
                var error = response.Error;
            }

            foreach (var coinBalance in response.Data.Balances)
            {
                if (coinBalance.Total > 0)
                {
                    userHeldCoins.Add(coinBalance);
                }
            }

            return userHeldCoins;
        }

        private async Task GetAllCoinsTradeHistory()
        {

        }
    }
}
