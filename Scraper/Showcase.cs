using Scraper.Model;
using System.Linq;
using System.Threading.Tasks;

namespace Scraper
{
    internal class Showcase
    {
        private readonly TestDb _db = new TestDb();

        public async Task ShowcaseMode()
        {
            await _db.Database.ExecuteSqlCommandAsync("DELETE FROM [Image] WHERE Id IN (SELECT Image FROM Selling WHERE Product IN (SELECT TOP (10) Id FROM Product))");
            await _db.Database.ExecuteSqlCommandAsync("DELETE FROM [Selling] WHERE Product IN (SELECT TOP (10) Id FROM Product)");

            var prods = _db.Product.Take(10).ToArray();
            var scrp = new SellingScraper(prods);
            await scrp.StartShowcasing();
        }
    }
}
