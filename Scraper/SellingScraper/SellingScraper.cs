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
        //var urlPtt = new Url("https://www.ptt.cc/bbs/MobileComm/search?q=iphone");
        //var doc = browser.OpenAsync(urlPtt);

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
                //new PchomeScraper(_browser, _db)
            };
        }

        public async Task StartScraping()
        {
            var tasks = Enumerable.Range(0, _scrapers.Length).Select(async i =>
            {
                var sellings = await _scrapers[i].Scrape();
                _db.Selling.AddRange(sellings);
            });

            await Task.WhenAll(tasks);
            await _db.SaveChangesAsync();
        }
    }
}
