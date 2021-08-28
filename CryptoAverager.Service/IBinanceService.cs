using System.Threading.Tasks;

namespace CryptoAverager.Service
{
    public interface IBinanceService
    {
        public Task CreateClient(string key, string secret);
    }
}
