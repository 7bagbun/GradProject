using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using AngleSharp;
using AngleSharp.Dom;
using Scraper.Model;

namespace Scraper
{
    internal class PcstoreScraper : ISellingScraper
    {
        private const float _price_interval = 0.15f;
        private const int _source_id = 1;

        private readonly IBrowsingContext _browser;
        private readonly TestDb _db;

        public PcstoreScraper(IBrowsingContext ctx, TestDb db)
        {
            _browser = ctx;
            _db = db;
        }

        public async Task<Selling[]> Scrape()
        {
            var prods = _db.Product.ToArray();
            var buffer = new List<Selling>();

            foreach (var p in prods)
            {
                //Using price gap to filter out unwanted sellings
                int pLow = (int)(p.RetailPrice * (1 - _price_interval));
                int pHigh = (int)(p.RetailPrice * (1 + _price_interval));
                string json = await RequestSellingData(p.Model, pLow, pHigh);
                buffer.AddRange(await ParseData(p.Model, json));

                await Task.Delay(500);
            }

            return buffer.ToArray();
        }

        private async Task<string> RequestSellingData(string kword, int priceL, int priceH)
        {
            kword = HttpHelper.UrlEncodedStringToBase64(kword);
            string link = "https://www.pcstore.com.tw/adm/api/get_search_data.php" +
                $"?store_k_word={kword}&slt_p_range_s={priceL}&slt_p_range_e={priceH}&st_sort=3";
            //st_sort=3 => order by lowest price

            var doc = await _browser.OpenAsync(link);
            string resp = doc.Body.Text();
            return resp;
        }

        private async Task<Selling[]> ParseData(string model, string json)
        {
            var obj = JsonConvert.DeserializeObject<JObject>(json);
            int count = obj["prod"].Count();
            var prod = _db.Product.FirstOrDefault(x => x.Model == model);
            var buffer = new Selling[count];

            for (int i = 0; i < count; i++)
            {
                int price = int.Parse(obj["prod"][i].Value<string>("price"),
                    System.Globalization.NumberStyles.AllowThousands);

                string title = obj["prod"][i].Value<string>("title");
                string imgLink = obj["prod"][i].Value<string>("mimg");

                if (price == 0 || title.Contains("另售"))
                {
                    continue;
                }

                buffer[i] = new Selling
                {
                    Title = title,
                    Price = price,
                    Link = obj["prod"][i].Value<string>("url").Substring(27),
                    Image1 = new Image()
                    { ImageContent = await HttpHelper.DownloadImageBytesAsync(imgLink) },

                    Source = _source_id,
                    Product1 = prod,
                    UpdatedTime = DateTime.Now
                };
            }

            return buffer.Where(x => x != null).ToArray();
        }
    }
}
