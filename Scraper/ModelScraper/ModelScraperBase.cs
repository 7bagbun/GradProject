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
                models = RemoveDuplicate(models);
                _db.Product.AddRange(models);
            }

            await _db.SaveChangesAsync();
        }

        private Product[] RemoveDuplicate(Product[] array)
        {
            var exist = _db.Product.Select(x => x.Model).ToArray();
            var map = new HashSet<string>(exist);

            for (int i = 0; i < array.Length; i++)
            {
                if (map.Contains(array[i].Model))
                {
                    array[i] = null;
                }
            }

            //remove duplicate in array
            for (int i = 0; i < array.Length; i++)
            {
                if (array[i] == null)
                    continue;

                for (int j = i + 1; j < array.Length; j++)
                {
                    if (array[j] == null ||
                         array[i].Model != array[j].Model) continue;

                    if (array[i].RetailPrice < array[j].RetailPrice)
                    {
                        array[j] = null;
                    }
                    else
                    {
                        array[i] = null;
                        break;
                    }
                }
            }

            return array.Where(x => x != null).ToArray();
        }
    }
}
