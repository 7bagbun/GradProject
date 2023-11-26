using System.Threading.Tasks;
using Scraper.Model;
using AngleSharp;
using System.Linq;
using System.Collections.Generic;
using Scraper.ModelScraper;

namespace Scraper
{
    internal interface IModelScraper
    {
        Task<Product[]> GetModels();
    }

    internal interface IBrandScraper
    {
        Task<Product[]> ParseModelData(IBrowsingContext ctx);
    }

    internal class ModelScraperBase
    {
        private readonly TestDb _db = new TestDb();
        private readonly IBrowsingContext _ctx;

        private readonly IModelScraper[] _modelScrapers;

        public ModelScraperBase()
        {
            var cfg = Configuration.Default.WithDefaultLoader();
            _ctx = BrowsingContext.New(cfg);

            _modelScrapers = new IModelScraper[]
            {
                //new DehumidMs(_ctx),
                //new HairDryerMs(_ctx),
                //new FridgeMs(_ctx),
                new WashMachineMS(_ctx),
            };
        }

        public async Task StartScrapingModel()
        {
            foreach (var item in _modelScrapers)
            {
                var models = await item.GetModels();
                _db.Product.AddRange(models);
            }

            await _db.SaveChangesAsync();
        }
    }
}
