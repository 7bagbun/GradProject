using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AngleSharp;
using AngleSharp.Dom;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Scraper.Model;

namespace Scraper
{
    internal class PchomeScraper : ISellingScraper
    {
        private const float _price_interval = 0.15f;
        private const int _source_id = 2;

        private readonly Product[] _prods;
        private readonly IBrowsingContext _browser;
        private readonly TestDb _db;

        public PchomeScraper(IBrowsingContext ctx, Product[] prods, TestDb db)
        {
            _db = db;
            _browser = ctx;
            _prods = prods;
        }

        public async Task<Selling[]> Scrape()
        {
            var buffer = new List<Selling>();

            foreach (var p in _prods)
            {
                //Using price gap to filter out unwanted sellings
                int pLow = (int)(p.RetailPrice * (1 - _price_interval));
                int pHigh = (int)(p.RetailPrice * (1 + _price_interval));
                string json = await RequestSellingData(p.Model, pLow, pHigh);
                buffer.AddRange(await ParseData(p.Id, json));

                await Task.Delay(500);
            }

            return buffer.ToArray();
        }

        private async Task<string> RequestSellingData(string kword, int priceL, int priceH)
        {
            kword = System.Net.WebUtility.UrlEncode(kword);
            string link = $"https://ecshweb.pchome.com.tw/search/v3.3/all/results?q={kword}"
                + $"&price={priceL}-{priceH}";

            var doc = await _browser.OpenAsync(link);
            string resp = doc.Body.Text();
            return resp;
        }

        private async Task<Selling[]> ParseData(int prodId, string json)
        {
            var obj = JsonConvert.DeserializeObject<JObject>(json);
            int count = obj["prods"] != null ? obj["prods"].Count() : 0;
            var buffer = new Selling[count];

            for (int i = 0; i < count; i++)
            {
                int price = int.Parse(obj["prods"][i].Value<string>("price"));

                string title = obj["prods"][i].Value<string>("name");
                string imgLink = obj["prods"][i].Value<string>("picS");
                var imageByte = HttpHelper.DownloadImageBytesAsync("https://cs-b.ecimg.tw" + imgLink);

                buffer[i] = new Selling
                {
                    Title = title,
                    Price = price,
                    Link = obj["prods"][i].Value<string>("Id"),
                    Image1 = new Image()
                    {
                        ImageContent = await imageByte,
                        LowresImage = ImageHelper.DownsizeImage(await imageByte)
                    },
                    Source = _source_id,
                    Product = prodId,
                    UpdatedTime = DateTime.Now
                };
            }

            return buffer;
        }

    }
}
