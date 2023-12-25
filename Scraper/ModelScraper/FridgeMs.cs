using AngleSharp;
using AngleSharp.Dom;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Scraper.Model;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Scraper.ModelScraper
{
    internal class FridgeMs : IModelScraper
    {
        private readonly IBrowsingContext _ctx;
        private readonly IBrandScraper[] _brands;

        public FridgeMs(IBrowsingContext ctx)
        {
            _ctx = ctx;
            _brands = new IBrandScraper[]
            {
                new PanaFridge(),
            };
        }

        public async Task<Product[]> GetModels()
        {
            var list = new List<Product>();

            foreach (var item in _brands)
            {
                list.AddRange(await item.ParseModelData(_ctx));
            }

            return list.ToArray();
        }

        private class PanaFridge : IBrandScraper
        {
            public async Task<Product[]> ParseModelData(IBrowsingContext ctx)
            {
                string data = await RequestModelData(ctx);
                var obj = JsonConvert.DeserializeObject<JArray>(data);
                var buffer = new Product[obj.Count];

                for (var i = 0; i < obj.Count; i++)
                {
                    string name = obj[i].Value<string>("Name");
                    string model = Regex.Match(name, @"(NR-[A-Z0-9]+-[A-Z0-9]+)|(NR-[A-Z0-9]+)").Value;

                    if (model == string.Empty)
                    {
                        continue;
                    }

                    var prod = new Product();
                    prod.Brand = "國際 Panasonic";
                    prod.ProductType = "冰箱";
                    prod.Model = model;
                    prod.RetailPrice = obj[i]["Price"].Value<int>("P");
                    prod.Token += Regex.Match(name, @"[\d\.]+((公升)|L)").Value.Replace("公升", "L") + " ";
                    prod.Token += Regex.Match(name, @".門").Value + " ";
                    prod.Token += Regex.Match(name, @"變頻").Value;
                    buffer[i] = prod;
                }

                return buffer.Where(x => x != null).ToArray();
            }

            public async Task<string> RequestModelData(IBrowsingContext ctx)
            {
                string url = "https://ecapi-cdn.pchome.com.tw/cdn/ecshop/prodapi/v2/store/DPAC1T/prod" +
                    "&offset=1&limit=36&fields=Name,Price&_callback=none";
                var resp = await ctx.OpenAsync(url);
                string json = resp.Body.Text();

                return json.Substring(9, json.Length - 57);
            }
        }
    }
}
