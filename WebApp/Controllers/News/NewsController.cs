using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApp.Models;

namespace WebApp.Controllers.News
{
    public class NewsController : Controller
    {
        private readonly TestDbEntities _db = new TestDbEntities();

        public ActionResult GetAllNews(int page = 1)
        {
            page--;
            var list = _db.News.OrderByDescending(x => x.CreatedDate).Skip(page * 4).Take(4).ToArray();
            if (list.Length < 4) //when last page does not have enough news
            {
                int offset = 4 - list.Length;
                list = _db.News.OrderByDescending(x => x.CreatedDate).Skip(page * 4 - offset).Take(4).ToArray();
            }

            var config = new JsonSerializerSettings() { DateFormatString = "yyyy/MM/dd hh:mm:ss" };
            string json = JsonConvert.SerializeObject(list, config);

            return Content(json, "application/json");
        }
        
        public ActionResult GetPageCount()
        {
            int count = (int)Math.Ceiling(_db.News.Count() / 4f);

            return Content($"{{\"pages\":{count}}}", "application/json");
        }
    }
}