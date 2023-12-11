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
    internal class TvMs : IModelScraper
    {
        private readonly IBrowsingContext _ctx;
        private readonly IBrandScraper[] _brands;

        public TvMs(IBrowsingContext ctx)
        {
            _ctx = ctx;
            _brands = new IBrandScraper[]
            {
                new SonyTv(),
                new SamsungTv()
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

        private class SonyTv : IBrandScraper
        {
            public async Task<Product[]> ParseModelData(IBrowsingContext ctx)
            {
                string data = await RequestModelData(ctx);
                var obj = JsonConvert.DeserializeObject<JArray>(data);
                var buffer = new Product[obj.Count];

                for (var i = 0; i < obj.Count; i++)
                {
                    string name = obj[i].Value<string>("Name");
                    string model = Regex.Match(name, @"[A-Z]+-[A-Z0-9]+").Value;

                    if (model == string.Empty)
                    {
                        continue;
                    }

                    var prod = new Product();
                    prod.Brand = "索尼 Sony";
                    prod.ProductType = "液晶電視";
                    prod.Type = 6;
                    prod.Model = model;
                    prod.RetailPrice = obj[i]["Price"].Value<int>("P");
                    prod.Token += name.IndexOf("mini", StringComparison.InvariantCultureIgnoreCase) >= 0 ? "mini " : "";
                    prod.Token += name.IndexOf("4K", StringComparison.InvariantCultureIgnoreCase) >= 0 ? "4K " : "";
                    prod.Token += name.IndexOf("8K", StringComparison.InvariantCultureIgnoreCase) >= 0 ? "8K " : "";
                    prod.Token += name.IndexOf("OLED", StringComparison.InvariantCultureIgnoreCase) >= 0 ? "OLED " : "";
                    prod.Token += Regex.Match(name, @"\d+(吋|型)").Value.Replace("型", "吋") + " ";
                    buffer[i] = prod;
                }

                return buffer.Where(x => x != null).ToArray();
            }

            public async Task<string> RequestModelData(IBrowsingContext ctx)
            {
                string url = "https://ecapi-cdn.pchome.com.tw/cdn/ecshop/prodapi/v2/store/DPADFX/prod" +
                    "&offset=1&limit=36&fields=Name,Price&_callback=none";
                var resp = await ctx.OpenAsync(url);
                string json = resp.Body.Text();

                return json.Substring(9, json.Length - 57);
            }
        }
        
        private class SamsungTv : IBrandScraper
        {
            public async Task<Product[]> ParseModelData(IBrowsingContext ctx)
            {
                string data = await RequestModelData(ctx);
                var obj = JsonConvert.DeserializeObject<JArray>(data);
                var buffer = new Product[obj.Count];

                for (var i = 0; i < obj.Count; i++)
                {
                    string name = obj[i].Value<string>("Name");
                    string model = Regex.Match(name, @"[A-Z]A[A-Z0-9]+XZW").Value;

                    if (model == string.Empty)
                    {
                        continue;
                    }

                    var prod = new Product();
                    prod.Brand = "三星 Samsung";
                    prod.ProductType = "液晶電視";
                    prod.Type = 6;
                    prod.Model = model;
                    prod.RetailPrice = obj[i]["Price"].Value<int>("P");
                    prod.Token += name.IndexOf("mini", StringComparison.OrdinalIgnoreCase) >= 0 ? "mini " : "";
                    prod.Token += name.IndexOf("4K", StringComparison.OrdinalIgnoreCase) >= 0 ? "4K " : "";
                    prod.Token += name.IndexOf("8K", StringComparison.OrdinalIgnoreCase) >= 0 ? "8K " : "";
                    prod.Token += name.IndexOf("OLED", StringComparison.OrdinalIgnoreCase) >= 0 ? "OLED " : "";
                    prod.Token += name.IndexOf("QLED", StringComparison.OrdinalIgnoreCase) >= 0 ? "QLED " : "";
                    prod.Token += Regex.Match(name, @"\d+吋").Value + " ";
                    buffer[i] = prod;
                }

                return buffer.Where(x => x != null).ToArray();
            }

            public async Task<string> RequestModelData(IBrowsingContext ctx)
            {
                string url = "https://ecapi-cdn.pchome.com.tw/cdn/ecshop/prodapi/v2/store/DPADFN/prod" +
                    "&offset=1&limit=36&fields=Name,Price&_callback=none";
                var resp = await ctx.OpenAsync(url);
                string json = resp.Body.Text();

                return json.Substring(9, json.Length - 57);
            }
        }
    }
}
