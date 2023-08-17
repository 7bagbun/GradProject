using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AngleSharp;
using AngleSharp.Dom;

namespace Scraper.Parser
{
    internal struct Article
    {
        public string Title;
        public string Url;
    }

    internal class ParserPtt
    {
        public static string ParseArticles(IDocument doc)
        {
            var div = doc.QuerySelectorAll(".r-ent").Select(x => x.Text());

            foreach (var item in div)
            {
                Console.WriteLine(item);
            }

            return "";
        }
    }
}
