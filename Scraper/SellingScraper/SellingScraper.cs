using System;
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
            var prods = _db.Product.ToArray();
            _browser = BrowsingContext.New(cfg);
            _scrapers = new ISellingScraper[]
            {
                new PcstoreScraper(_browser, prods, _db),
                new PchomeScraper(_browser, prods, _db),
                new MomoScraper(prods, _db),
            };
        }

        //for showcase purpose
        public SellingScraper(Product[] prods)
        {
            var cfg = Configuration.Default.WithDefaultLoader();
            _browser = BrowsingContext.New(cfg);
            _scrapers = new ISellingScraper[]
            {
                new PcstoreScraper(_browser, prods, _db),
                new PchomeScraper(_browser, prods, _db),
                new MomoScraper(prods, _db),
            };
        }

        //for showcase purpose
        public async Task StartShowcasing()
        {
            try
            {
                var tasks = Enumerable.Range(0, _scrapers.Length).Select(async i =>
                {
                    var sellings = await _scrapers[i].Scrape();
                    _db.Selling.AddRange(sellings);
                });

                await Task.WhenAll(tasks);
                await _db.SaveChangesAsync();
                await UpdatePriceHistory();
            }
            catch (Exception ex)
            {
                ExceptionLogger.LogException(ex);
                throw;
            }
        }

        public async Task StartScraping()
        {
            try
            {
                var tasks = Enumerable.Range(0, _scrapers.Length).Select(async i =>
                {
                    var sellings = await _scrapers[i].Scrape();
                    _db.Selling.AddRange(sellings);
                });

                await Task.WhenAll(tasks);
                await ClearSellings();
                await _db.SaveChangesAsync();
                await UpdatePriceHistory();
            }
            catch (Exception ex)
            {
                ExceptionLogger.LogException(ex);
                throw;
            }
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
                    isCheaper = true;
                }

                x.PreviousLow = x.CurrentLow;
                x.CurrentLow = lowest;

                _db.PriceHistory.Add(new PriceHistory
                {
                    Product = x.Id,
                    Price = lowest,
                    UpdatedTime = System.DateTime.Now
                });
            });

            await _db.SaveChangesAsync();

            if (isCheaper)
            {
                await HttpHelper.SendRequest("http://localhost:43369/priceHistory/checkPrice");
            }
        }

        public async Task ClearSellings()
        {
            await _db.Database.ExecuteSqlCommandAsync("DELETE FROM [Image] WHERE Id IN (SELECT Image FROM Selling)");
            await _db.Database.ExecuteSqlCommandAsync("TRUNCATE TABLE Selling");
        }
    }
}
