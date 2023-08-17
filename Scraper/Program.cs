using System.Threading.Tasks;

namespace Scraper
{
    public class Program
    {
        static async Task Main(string[] args)
        {
            var ms = new ModelScraper();
            await ms.StartScrapingModel();

            var scrp = new SellingScraper();
            await scrp.StartScraping();
        }
    }
}
