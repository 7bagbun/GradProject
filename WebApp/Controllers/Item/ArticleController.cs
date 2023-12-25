using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApp.Models;

namespace WebApp.Controllers.Item
{
    public class ArticleController : Controller
    {
        private readonly TestDbEntities _db = new TestDbEntities();

        public ActionResult GetById(int productId)
        {
            var articles = _db.Article.Include("Source1").Where(x => x.Product == productId).Select(x =>
                new
                {
                    link = x.Source1.Domain + x.Link,
                    title = x.Title,
                    content = x.Content,
                    image = "/Assets/Images/" + x.Source1.ImageName
                }).ToArray();

            var json = JsonConvert.SerializeObject(articles);
            return Content(json, "application/json");
        }
    }
}