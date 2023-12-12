using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Scraper.Model;

namespace Scraper
{
    internal class MomoScraper : ISellingScraper
    {
        private const float _price_interval = 0.15f;
        private const int _source_id = 5;

        private readonly Product[] _prods;
        private readonly TestDb _db;
        private static HttpClient _client;

        public MomoScraper(Product[] prods, TestDb db)
        {
            _db = db;
            _client = new HttpClient();
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

                await Task.Delay(750);
            }

            return buffer.ToArray();
        }

        private async Task<string> RequestSellingData(string kword, int priceL, int priceH)
        {
            string link = "https://apisearch.momoshop.com.tw/momoSearchCloud/moec/textSearch";
            string json = $"{{\"host\":\"momoshop\",\"flag\":\"searchEngine\",\"data\":{{\"specialGoodsType\":\"\",\"isBrandSeriesPage\":false,\"authorNo\":\"\",\"originalCateCode\":\"\",\"cateType\":\"\",\"searchValue\":\"{kword}\",\"cateCode\":\"\",\"cateLevel\":\"-1\",\"cp\":\"N\",\"NAM\":\"N\",\"first\":\"N\",\"freeze\":\"N\",\"superstore\":\"N\",\"tvshop\":\"N\",\"china\":\"N\",\"tomorrow\":\"N\",\"stockYN\":\"N\",\"prefere\":\"N\",\"threeHours\":\"N\",\"video\":\"N\",\"cycle\":\"N\",\"cod\":\"N\",\"superstorePay\":\"N\",\"showType\":\"chessboardType\",\"curPage\":\"1\",\"priceS\":\"{priceL}\",\"priceE\":\"{priceH}\",\"searchType\":\"1\",\"reduceKeyword\":\"\",\"isFuzzy\":\"0\",\"rtnCateDatainfo\":{{\"cateCode\":\"\",\"cateLv\":\"-1\",\"keyword\":\"{kword}\",\"curPage\":\"1\",\"historyDoPush\":false,\"timestamp\":1698996058158}},\"flag\":2018,\"serviceCode\":\"MT01\",\"addressSearchData\":{{}},\"adSource\":\"tenmax\"}}}}";

            var resp = await _client.PostAsync(link,
                 new StringContent(json, System.Text.Encoding.UTF8, "application/json"));

            var strResp = await resp.Content.ReadAsStringAsync();
            var goods = JToken.Parse(strResp)["rtnSearchData"]["goodsInfoList"];
            return goods?.ToString() ?? "[]";
        }

        private async Task<Selling[]> ParseData(int prodId, string json)
        {
            var jsonArr = JsonConvert.DeserializeObject<JArray>(json);
            int count = jsonArr.Count();

            if (count < 1)
            {
                return new Selling[0];
            }

            var buffer = new Selling[count];

            for (int i = 0; i < count; i++)
            {
                if (int.TryParse(jsonArr[i]["SALE_PRICE"].Value<string>(), out int price))
                {
                    string title = jsonArr[i].Value<string>("goodsName");
                    string imgLink = jsonArr[i].Value<string>("imgUrl");
                    var imageByte = HttpHelper.DownloadImageBytesAsync(imgLink);

                    buffer[i] = new Selling
                    {
                        Title = title,
                        Price = price,
                        Link = jsonArr[i]["action"].Value<string>("actionValue"),
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
            }

            return buffer.Where(x => x != null).ToArray();
        }
    }
}
