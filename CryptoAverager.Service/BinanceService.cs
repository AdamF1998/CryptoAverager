using Binance.Net;
using Binance.Net.Objects;
using Binance.Net.Objects.Spot.MarketData;
using Binance.Net.Objects.Spot.SpotData;
using CryptoExchange.Net.Authentication;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
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
                entry = new BinanceClient(new BinanceClientOptions() 
                { 
                    ApiCredentials = new ApiCredentials(secureKeyString, secureSecretString) 
                });

                // Give the user 60 minutes before having to reauthenticate with the api again
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromMinutes(60));

                _cache.Set(key, entry, cacheEntryOptions);
            }
        }

        public async void CalculateCoinAverages(string apiKey)
        {
            if (string.IsNullOrEmpty(apiKey))
            {
                throw new ArgumentException(nameof(apiKey));
            }

            _cache.TryGetValue(apiKey, out BinanceClient binanceClient);

            if (binanceClient == null)
            {
                throw new ArgumentNullException(nameof(binanceClient));
            }

            //ToDo: This method will be returning the trades for each coin
            await FindAllTradesForEachCoin(binanceClient);
        }

        private async Task FindAllTradesForEachCoin(BinanceClient binanceClient)
        {
            // Find out all the coins the user has a balance for
            var coins = await GetAllCoinsWithABalance(binanceClient);

            IEnumerable<BinanceSymbol> binanceSymbols = null;

            // Get all binance pairs
            var exchangeInfoResult = await binanceClient.Spot.System.GetExchangeInfoAsync();

            if (exchangeInfoResult.Success)
            {
                binanceSymbols = exchangeInfoResult.Data.Symbols;
            }

            //ToDo: Store the stable coins and fiat currencies somewhere else. I also need to pull this data from somewhere.
            string[] stableCoins = { "USDT", "BUSD", "USDC" };

            string[] fiatCurrency = { "GBP" };

            // Find all the pairs for the coins that the user has a balance for
            var matchingSymbols = FindAllMatchingSymbols(coins, binanceSymbols);

            // We want to remove any stablecoins and fiat currency
            matchingSymbols.RemoveAll(x =>
                stableCoins.Contains(x.GetType().GetProperty("Asset").GetValue(x).ToString()) ||
                fiatCurrency.Contains(x.GetType().GetProperty("Asset").GetValue(x).ToString()));

            // Go through each coin, get the trade history and then calculate the average purchase price
            foreach (var symbol in matchingSymbols)
            {
                var trades = await binanceClient.Spot.Order
                    .GetUserTradesAsync(symbol.GetType().GetProperty("Name").GetValue(symbol).ToString());

                if (trades.Data.Any())
                {
                    //ToDo: I need to do something with this data
                }
            }
        }

        private async Task<List<BinanceBalance>> GetAllCoinsWithABalance(BinanceClient client)
        {
            List<BinanceBalance> userHeldCoins = new List<BinanceBalance>();

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

        private List<object> FindAllMatchingSymbols(List<BinanceBalance> coins, IEnumerable<BinanceSymbol> binanceSymbols)
        {
            
            var matchingSymbols = from x in coins
                                  join y in binanceSymbols on x.Asset equals y.BaseAsset
                                  select new
                                  {
                                      x.Asset,
                                      y.Name
                                  };

            return matchingSymbols.ToList<object>();
        }
    }
}
