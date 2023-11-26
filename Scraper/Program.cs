using AngleSharp.Dom;
using System;
using System.Threading.Tasks;
using AngleSharp;
using System.Linq;

namespace Scraper
{
    public class Program
    {
        static async Task Main(string[] args)
        {
            //var ms = new ModelScraperBase();
            //await ms.StartScrapingModel();

            //var scrp = new SellingScraper();
            //await scrp.StartScraping();

            //var artScrp = new ArticleScraper();
            //await artScrp.StartScraping();

            bool resp = await HttpHelper.SendRequest("https://localhost:44369/priceHistory/checkPrice");
            await Console.Out.WriteLineAsync(resp ? "y" : "f");

            Console.Read();
        }
    }
}
