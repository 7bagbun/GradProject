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

        private readonly Product[] _prods;
        private readonly IBrowsingContext _browser;
        private readonly TestDb _db;

        public PcstoreScraper(IBrowsingContext ctx, Product[] prods, TestDb db)
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
                string query = p.Brand.Split(' ')[1] + " " + p.Model;
                string json = await RequestSellingData(query, pLow, pHigh);
                var prods = await ParseData(p.Id, json);
                buffer.AddRange(prods);

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

        private async Task<Selling[]> ParseData(int prodId, string json)
        {
            var obj = JsonConvert.DeserializeObject<JObject>(json);
            int count = obj["prod"].Count();
            var buffer = new Selling[count];

            for (int i = 0; i < count; i++)
            {
                int price = int.Parse(obj["prod"][i].Value<string>("price"),
                    System.Globalization.NumberStyles.AllowThousands);

                string title = obj["prod"][i].Value<string>("title");
                string imgLink = obj["prod"][i].Value<string>("mimg");
                var imageBytes = HttpHelper.DownloadImageBytesAsync(imgLink);

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
                    {
                        ImageContent = await imageBytes,
                        LowresImage = ImageHelper.DownsizeImage(await imageBytes),
                    },
                    Source = _source_id,
                    Product = prodId,
                    UpdatedTime = DateTime.Now
                };

                buffer[i].Image1.LowresImage = ImageHelper.DownsizeImage(buffer[i].Image1.ImageContent);
            }

            return buffer.Where(x => x != null).ToArray();
        }
    }
}
