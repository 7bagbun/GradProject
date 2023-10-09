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

        public ActionResult GetAllNews()
        {
            var list = _db.News.OrderBy(x => x.CreatedDate).ToArray();
            var config = new JsonSerializerSettings() { DateFormatString = "yyyy/MM/dd hh:mm:ss" };
            string json = JsonConvert.SerializeObject(list, config);

            return Content(json, "application/json");
        }
    }
}