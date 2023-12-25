using AngleSharp;
using AngleSharp.Dom;
using Scraper.Model;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Scraper
{
    interface IArticleScraper
    {
        Task<Article[]> Scrape(Product[] model);
    }

    internal class ArticleScraper
    {
        private readonly TestDb _db = new TestDb();
        private readonly IBrowsingContext _browser;
        private readonly IArticleScraper[] _scrapers;

        public ArticleScraper()
        {
            var cfg = Configuration.Default.WithDefaultLoader();
            _browser = BrowsingContext.New(cfg);
            _scrapers = new IArticleScraper[]
            {
                new PttCs(_browser),
            };
        }

        public async Task StartScraping()
        {
            var prods = _db.Product.ToArray();

            var tasks = Enumerable.Range(0, _scrapers.Length).Select(async i =>
            {
                var articles = await _scrapers[i].Scrape(prods);
                articles = RemoveDuplicate(articles);
                _db.Article.AddRange(articles);
            });

            await Task.WhenAll(tasks);
            await _db.SaveChangesAsync();
        }

        private Article[] RemoveDuplicate(Article[] array)
        {
            var exist = _db.Article.Select(x => x.Title).ToArray();
            var map = new HashSet<string>(exist);

            for (int i = 0; i < array.Length; i++)
            {
                if (map.Contains(array[i].Title))
                {
                    array[i] = null;
                }
            }

            return array.Where(x => x != null).ToArray();
        }

        private class PttCs : IArticleScraper
        {
            private const int _source_id = 6;
            private readonly IBrowsingContext _browser;

            public PttCs(IBrowsingContext ctx)
            {
                _browser = ctx;
            }

            public async Task<Article[]> Scrape(Product[] model)
            {
                var artList = new List<Article>(20);

                foreach (var item in model)
                {
                    artList.AddRange(await SearchArticle(item.Id, item.Model));
                }

                return artList.ToArray();
            }

            private async Task<Article[]> SearchArticle(int productId, string kword)
            {
                kword = ProcessKword(kword);
                var urlApp = new Url("https://www.ptt.cc/bbs/E-appliance/search?q=" + kword);
                var urlShop = new Url("https://www.ptt.cc/bbs/E-shopping/search?q=" + kword);

                var appTask = _browser.OpenAsync(urlApp);
                var shopTask = _browser.OpenAsync(urlShop);

                var app = (await appTask).QuerySelectorAll(".title a").Where(x => !FilterArticle(x.Text())).Select(
                    x => new Article
                    {
                        Product = productId,
                        Source = _source_id,
                        Title = x.Text().Substring(5),
                        Link = x.Attributes["href"].Value.Substring(1),
                        Content = SelectContent(x.Attributes["href"].Value.Substring(1)).Result
                    });

                await Task.Delay(300);

                var shop = (await shopTask).QuerySelectorAll(".title a").Where(x => !FilterArticle(x.Text())).Select(
                    x => new Article
                    {
                        Product = productId,
                        Source = _source_id,
                        Title = x.Text().Substring(5),
                        Link = x.Attributes["href"].Value.Substring(1),
                        Content = SelectContent(x.Attributes["href"].Value.Substring(1)).Result
                    });

                appTask.Result.Close();
                shopTask.Result.Close();
                await Task.Delay(300);
                return app.Concat(shop).ToArray();
            }

            public async Task<string> SelectContent(string url)
            {
                var articleUrl = new Url("https://www.ptt.cc/" + url);
                var doc = await _browser.OpenAsync(articleUrl);

                foreach (var item in doc.QuerySelectorAll(".article-metaline, .article-metaline-right, .push, .richcontent"))
                {
                    item.Remove();
                }

                string content = doc.QuerySelector("#main-content").Text().Split('-')[0];
                doc.Close();

                int length = content.Length > 200 ? 200 : content.Length;
                return content.Substring(content.Length - length, length) + "...";
            }

            public bool FilterArticle(string title)
            {
                title = title.ToLower();

                return
                    title.Contains("水桶")
                    || title.Contains("phone");
            }

            private string ProcessKword(string kword)
            {
                var arr = kword.Split('-');
                if (arr.Length > 1)
                {
                    return arr[1];
                }
                else
                {
                    return kword;
                }
            }
        }
    }
}
