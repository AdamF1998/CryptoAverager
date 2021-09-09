using Binance.Net;

namespace CryptoAverager.Service
{
    public interface IBinanceService
    {
        public void CreateClient(string key, string secret);

        public void CalculateCoinAverages();
    }
}
