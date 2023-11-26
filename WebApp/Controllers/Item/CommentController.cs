using Microsoft.Ajax.Utilities;
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
            //identify if login user is also who wrote comment
            int userId = Session["userId"] == null ? -1 : (int)Session["userId"];

            var data = _db.Comment.Include("Member").OrderByDescending(x => x.CreatedDate).Where(x => x.Product == id);
            var comments = new CommentViewModel[data.Count()];
            int index = 0;
            data.ForEach(x =>
            {
                comments[index++] = new CommentViewModel
                {
                    Author = x.Member.Username,
                    Rating = x.Rating,
                    Content = x.Content,
                    IsAuthor = x.Author == userId,
                    CreatedAt = x.CreatedDate
                };
            });

            var dateSettings = new JsonSerializerSettings() { DateFormatString = "yyyy/MM/dd HH:mm:ss" };
            string json = JsonConvert.SerializeObject(comments, dateSettings);
            return Content(json, "application/json");
        }

        public ActionResult GetByModel(string model)
        {
            int? id = _db.Product.First(x => x.Model == model)?.Id;
            if (id == null) return Content(null, "application/json");

            var comments = _db.Comment.Where(x => x.Product == id);
            var dateSettings = new JsonSerializerSettings() { DateFormatString = "yyyy/MM/dd HH:mm:ss" };
            string json = JsonConvert.SerializeObject(comments, dateSettings);
            return Content(json, "application/json");
        }

        [HttpPost]
        public ActionResult Create(Comment comment)
        {
            if (Session["userId"] == null)
            {
                return RedirectToAction("NoLogin", "Redirect");
            }

            if (comment.Rating < 1 || comment.Rating > 5
                || comment.Content == null || comment.Content.Length > 500)
            {
                return new HttpStatusCodeResult(400);
            }

            int userId = (int)Session["userId"];
            if (_db.Comment.Any(x => x.Product == comment.Product && x.Author == userId))
            {
                return RedirectToAction("Error", "Redirect", new { msg = "您已評論過此產品" });
            }

            comment.Author = userId;
            comment.CreatedDate = DateTime.Now;
            _db.Comment.Add(comment);
            _db.SaveChanges();

            return RedirectToAction("List", "Item", new { id = comment.Product, tab = "comment"});
        }

        [HttpPost]
        public ActionResult Edit(Comment comment)
        {
            if (Session["userId"] == null)
            {
                return new HttpStatusCodeResult(401);
            }

            int userId = (int)Session["userId"];
            var target = _db.Comment.FirstOrDefault(x => x.Product == comment.Product && x.Author == userId);

            if (target == null)
            {
                return RedirectToAction("Error", "Redirect", new { msg = "您要編輯的評論不存在" });
            }

            target.Rating = comment.Rating;
            target.Content = comment.Content;
            _db.SaveChangesAsync();

            return RedirectToAction("List", "Item", new { id = comment.Product, tab = "comment" });
        }

        [HttpPost]
        public ActionResult Delete(int commentId)
        {
            if (Session["userId"] == null)
            {
                return new HttpStatusCodeResult(401);
            }

            var comment = _db.Comment.FirstOrDefault(x => x.Id == commentId);

            if (comment == null)
            {
                return RedirectToAction("Error", "Redirect", new { msg = "您要刪除的評論不存在" });
            }
            else if (comment.Author != (int)Session["userId"])
            {
                return new HttpStatusCodeResult(401);
            }

            _db.Comment.Remove(comment);
            _db.SaveChanges();

            return Content("{\"isSucceed\":true}", "application/json");
        }
    }
}