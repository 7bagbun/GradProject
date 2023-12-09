using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using AngleSharp;
using AngleSharp.Dom;
using Scraper.Model;

namespace Scraper
{
    internal class DehumidMs : IModelScraper
    {
        private readonly IBrowsingContext _ctx;
        private readonly IBrandScraper[] _brands;

        public DehumidMs(IBrowsingContext ctx)
        {
            _ctx = ctx;
            _brands = new IBrandScraper[]
            {
                new LgDehumidfier(),
                new MitDehumidfier()
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

        private class LgDehumidfier : IBrandScraper
        {
            public async Task<Product[]> ParseModelData(IBrowsingContext ctx)
            {
                string data = await RequestModelData(ctx);
                var obj = JsonConvert.DeserializeObject<JArray>(data);
                var buffer = new Product[obj.Count];

                for (var i = 0; i < obj.Count; i++)
                {
                    string name = obj[i].Value<string>("Name");
                    string model = Regex.Match(name, @"[A-Z0-9]{7,}").Value;

                    if (model == string.Empty)
                    {
                        continue;
                    }

                    var prod = new Product();
                    prod.Brand = "樂金 LG";
                    prod.ProductType = "除濕機";
                    prod.Model = model;
                    prod.RetailPrice = obj[i]["Price"].Value<int>("P");
                    prod.Token += Regex.Match(name, @"[\d\.]+((公升)|L)").Value.Replace("公升", "L");
                    buffer[i] = prod;
                }

                return buffer.Where(x => x != null).ToArray();
            }

            public async Task<string> RequestModelData(IBrowsingContext ctx)
            {
                string url = "https://ecapi-cdn.pchome.com.tw/cdn/ecshop/prodapi/v2/store/DMBQ07/prod" +
                    "&offset=1&limit=36&fields=Name,Price&_callback=none";
                var resp = await ctx.OpenAsync(url);
                string json = resp.Body.Text();

                return json.Substring(9, json.Length - 57);
            }
        }

        private class MitDehumidfier : IBrandScraper
        {
            public async Task<Product[]> ParseModelData(IBrowsingContext ctx)
            {
                string data = await RequestModelData(ctx);
                var obj = JsonConvert.DeserializeObject<JArray>(data);
                var buffer = new Product[obj.Count];

                for (var i = 0; i < obj.Count; i++)
                {
                    string title = obj[i].Value<string>("Name");
                    string model = Regex.Match(title, @"MJ-\w+((-TW)|)").Value;

                    if (model == string.Empty)
                    {
                        continue;
                    }

                    var prod = new Product();
                    prod.Brand = "三菱 MITSUBISHI";
                    prod.ProductType = "除濕機";
                    prod.Model = model;
                    prod.RetailPrice = obj[i]["Price"].Value<int>("P");
                    prod.Token += Regex.Match(title, @"[\d\.]+((公升)|L)")
                                    .Value.Replace("公升", "L");

                    buffer[i] = prod;
                }

                return buffer.Where(x => x != null).ToArray();
            }

            public async Task<string> RequestModelData(IBrowsingContext ctx)
            {
                string url = "https://ecapi-cdn.pchome.com.tw/cdn/ecshop/prodapi/v2/store/DMBQ02/prod" +
                    "&offset=1&limit=36&fields=Name,Price&_callback=none";

                var resp = await ctx.OpenAsync(url);
                string json = resp.Body.Text();

                return json.Substring(9, json.Length - 57);
            }
        }
    }
}
