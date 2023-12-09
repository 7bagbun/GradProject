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
            if (args.Length == 0)
            {
                await Console.Out.WriteLineAsync("%Usage%: scraper -[params]");
                return;
            }

            if (args[0].Contains("m"))
            {
                var ms = new ModelScraperBase();
                await ms.StartScrapingModel();
            }

            if (args[0].Contains("s"))
            {
                var scrp = new SellingScraper();
                await scrp.StartScraping();
            }

            if (args[0].Contains("a"))
            {
                var artScrp = new ArticleScraper();
                await artScrp.StartScraping();
            }

            if (args[0].Contains("S"))
            {
                await Console.Out.WriteLineAsync("Running in showcasing mode...");
                var sc = new Showcase();
                await sc.ShowcaseMode();
            }
        }
    }
}
