using RealTimePrices.Database;
using RealTimePrices.Models;
using System.Collections.Concurrent;
using Tinkoff.Trading.OpenApi.Network;

namespace RealTimePrices
{
    public class PriceProvider: IDisposable
    {
        
        private ConcurrentDictionary<string, Price> Prices { get; init; } = new();
        private Context context { get; init; }
        private DB db { get; init; }
        private ILogger _logger { get; init; }

//============================================================================================================//

        public PriceProvider(ILogger<PriceProvider> logger)
        {
            _logger = logger;

            var a = ConnectionFactory.GetConnection(Environment.GetEnvironmentVariable("TINKOFF_TOKEN"));
            context = a.Context;
            db = new DB();

            Update().Wait();
            Task.Run(() => Wrap(Update, "Prices", 60000));
        }

        public Dictionary<string, Price> GetByFigi(IEnumerable<string> figis)
        {
            var res = new Dictionary<string, Price>();
            foreach (var figi in figis)
            {
                if (Prices.TryGetValue(figi, out var price))
                {
                    res.Add(figi, price);
                }
            }
            return res;
        }

        public Dictionary<string, Price> GetByTicker(IEnumerable<string> tickers)
        {
            var res = new Dictionary<string, Price>();
            foreach (var ticker in tickers)
            {
                var figi = db.Papers.Where(x => x.Ticker == ticker.ToUpper()).Select(x => x.Figi).FirstOrDefault();
                if (figi != null)
                {
                    if (Prices.TryGetValue(figi, out var price))
                    {
                        res.Add(ticker.ToUpper(), price);
                    }
                };
            }
            return res;
        }

//============================================================================================================//
        
        private async Task Update()
        {
            var _db = new DB();
            foreach (var figi in _db.Papers.Select(x => x.Figi))
            {
                await Task.Delay(100);
                var price = await GetPriceByFigi(figi);
                if (price == null) continue;
                Prices.AddOrUpdate(figi, price, (k, o) => price);
            }
        }

        private async Task<Price?> GetPriceByFigi(string figi)
        {
            var paper = db.Papers.Where(x => x.Figi == figi).FirstOrDefault();
            if (paper == null) return null;
            var a = await context.MarketOrderbookAsync(figi, 1);
            var price = a.LastPrice;
            return new Price(paper.Currency, price);
        }

        private async Task Wrap(Func<Task> todo, string loggerInfo, int period)
        {
            while (true)
            {
                await Task.Delay(period);
                _logger.LogInformation($"Start updating {loggerInfo}");
                await todo();
                _logger.LogInformation($"{loggerInfo} updated");
            }
        }

        public void Dispose()
        {
            context.Dispose();
            db.Dispose();
        }
    }
}
