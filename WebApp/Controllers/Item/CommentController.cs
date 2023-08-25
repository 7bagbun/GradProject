using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApp.Models;

namespace WebApp.Controllers.Item
{
    public class CommentController : Controller
    {
        private readonly TestDbEntities _db = new TestDbEntities();

        public ActionResult GetById(int id)
        {
            var comments = _db.Comment.Where(x => x.Product == id);
            string json = JsonConvert.SerializeObject(comments);
            return Content(json, "application/json");
        }

        public ActionResult GetByModel(string model)
        {
            int? id = _db.Product.First(x => x.Model == model)?.Id;
            if (id == null) return Content(null, "application/json");

            var comments = _db.Comment.Where(x => x.Product == id);
            string json = JsonConvert.SerializeObject(comments);
            return Content(json, "application/json");
        }

        [HttpPost]
        public ActionResult Create(Comment comment)
        {
            if (comment.Rating < 0 || comment.Rating > 5)
            {
                return new HttpStatusCodeResult(400);
            }

            _db.Comment.Add(comment);
            _db.SaveChanges();
            return Content("{\"Succeed\":true}", "application/json");
        }
    }
}