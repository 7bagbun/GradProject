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

            var data = _db.Comment.Include("Member").Where(x => x.Product == id);
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
            try
            {
                if (Session["userId"] == null)
                {
                    return RedirectToAction("nologin", "error");
                }

                if (comment.Rating < 1 || comment.Rating > 5
                    || comment.Content == null || comment.Content.Length > 500)
                {
                    return new HttpStatusCodeResult(400);
                }

                comment.Author = (int)Session["userId"];
                comment.CreatedDate = DateTime.Now;
                _db.Comment.Add(comment);
                _db.SaveChanges();
                return Content("{\"Succeed\":true}", "application/json");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new HttpStatusCodeResult(500);
            }
        }

        [HttpPost]
        public ActionResult DeleteComment(int commentId)
        {
            if (Session["userId"] == null)
            {
                return new HttpStatusCodeResult(401);
            }
            
            var comment = _db.Comment.FirstOrDefault(x => x.Id == commentId);

            if (comment == null)
            {
                return RedirectToAction("CustomMessage", "Error", new { msg = "您要刪除的評論不存在" });
            }
            else if (comment.Author != (int)Session["userId"])
            {
                return new HttpStatusCodeResult(401);
            }

            return Content("{\"IsSucceed\":true}");
        }
    }
}