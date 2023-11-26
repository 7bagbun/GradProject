using System.Linq;
using System.Threading.Tasks;
using AngleSharp;
using Scraper.Model;

namespace Scraper
{
    interface ISellingScraper
    {
        Task<Selling[]> Scrape();
    }

    internal class SellingScraper
    {
        private readonly TestDb _db = new TestDb();
        private readonly IBrowsingContext _browser;
        private readonly ISellingScraper[] _scrapers;

        public SellingScraper()
        {
            var cfg = Configuration.Default.WithDefaultLoader();
            _browser = BrowsingContext.New(cfg);
            _scrapers = new ISellingScraper[]
            {
                new PcstoreScraper(_browser, _db),
                new PchomeScraper(_browser, _db),
                new MomoScraper(_db),
            };
        }

        public async Task StartScraping()
        {
            await ClearSellings();

            var tasks = Enumerable.Range(0, _scrapers.Length).Select(async i =>
            {
                var sellings = await _scrapers[i].Scrape();
                _db.Selling.AddRange(sellings);
            });

            await Task.WhenAll(tasks);
            await _db.SaveChangesAsync();
            await UpdatePriceHistory();
        }

        private async Task UpdatePriceHistory()
        {
            var prods = _db.Product.Include("Selling")
                .Where(x => x.Selling.FirstOrDefault() != null).ToList();

            bool isCheaper = false;

            prods.ForEach(x =>
            {
                int lowest = x.Selling.Min(t => t.Price);

                if (x.CurrentLow > lowest)
                {
                    x.PreviousLow = x.CurrentLow;
                    x.CurrentLow = lowest;
                    isCheaper = true;
                }

                _db.PriceHistory.Add(new PriceHistory
                {
                    Product = x.Id,
                    Price = lowest,
                    UpdatedTime = System.DateTime.Now
                });
            });

            if (isCheaper)
            {
                await HttpHelper.SendRequest("http://localhost:43369/priceHistory/checkPrice");
            }

            await _db.SaveChangesAsync();
        }

        private async Task ClearSellings()
        {
            await _db.Database.ExecuteSqlCommandAsync("DELETE FROM [Image] WHERE Id IN (SELECT Image FROM Selling)");
            await _db.Database.ExecuteSqlCommandAsync("TRUNCATE TABLE Selling");
        }
    }
}
