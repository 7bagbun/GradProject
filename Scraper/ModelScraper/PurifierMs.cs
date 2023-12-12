using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using AngleSharp;
using AngleSharp.Dom;
using Scraper.Model;
using System;

namespace Scraper.ModelScraper
{
    internal class PurifierMs : IModelScraper
    {
        private readonly IBrowsingContext _ctx;
        private readonly IBrandScraper[] _brands;

        public PurifierMs(IBrowsingContext ctx)
        {
            _ctx = ctx;
            _brands = new IBrandScraper[]
            {
                new DysonPurifier(),
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
        
        private class DysonPurifier : IBrandScraper
        {
            public async Task<Product[]> ParseModelData(IBrowsingContext ctx)
            {
                string data = await RequestModelData(ctx);
                var obj = JsonConvert.DeserializeObject<JArray>(data);
                var buffer = new Product[obj.Count];

                for (var i = 0; i < obj.Count; i++)
                {
                    string name = obj[i].Value<string>("Name");
                    string model = Regex.Match(name, @"[A-Z]{2}[A-Z0-9]{2}").Value;

                    if (model == string.Empty)
                    {
                        continue;
                    }

                    var prod = new Product();
                    prod.Brand = "戴森 Dyson";
                    prod.ProductType = "空氣清淨機";
                    prod.Type = 8;
                    prod.Model = model;
                    prod.RetailPrice = obj[i]["Price"].Value<int>("P");
                    prod.Token += name.IndexOf("甲醛偵測", StringComparison.OrdinalIgnoreCase) >= 0 ? "甲醛偵測" : "";
                    buffer[i] = prod;
                }

                return buffer.Where(x => x != null).ToArray();
            }

            public async Task<string> RequestModelData(IBrowsingContext ctx)
            {
                string url = "https://ecapi-cdn.pchome.com.tw/cdn/ecshop/prodapi/v2/store/DMAUCR/prod" +
                    "&offset=1&limit=36&fields=Name,Price&_callback=none";
                var resp = await ctx.OpenAsync(url);
                string json = resp.Body.Text();

                return json.Substring(9, json.Length - 57);
            }
        }
    }
}
