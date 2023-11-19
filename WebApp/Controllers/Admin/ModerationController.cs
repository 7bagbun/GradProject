﻿using System;
using System.Linq;
using System.Web.Mvc;
using WebApp.Models;

namespace WebApp.Controllers.Admin
{
    public class ModerationController : Controller
    {
        private readonly TestDbEntities _db = new TestDbEntities();

        [HttpPost]
        public ActionResult SetSuspend(int id, bool state)
        {
            if (Session["admin"] == null)
            {
                return new HttpStatusCodeResult(401);
            }

            var target = _db.Member.FirstOrDefault(x => x.Id == id);

            if (target == null)
            {
                return Content("{\"isSuceed\":false,\"msg\":\"無此會員\"}");
            }

            target.Suspended = state;
            _db.SaveChanges();

            return Content("{\"isSucceed\":true}", "application/json");
        }

        [HttpPost]
        public ActionResult DeleteComment(int commentId)
        {
            if (Session["admin"] == null)
            {
                return new HttpStatusCodeResult(401);
            }

            var comment = _db.Comment.FirstOrDefault(x => x.Id == commentId);

            if (comment == null)
            {
                return Content("{\"isSucceed\":false,\"msg\":\"您要刪除的評論不存在\"}", "application/json");
            }

            _db.Comment.Remove(comment);
            _db.SaveChanges();

            return Content("{\"isSucceed\":true}", "application/json");
        }

        public ActionResult EditAnnouncement(Models.News news)
        {
            if (Session["admin"] == null)
            {
                return new HttpStatusCodeResult(401);
            }

            var target = _db.News.FirstOrDefault(x => x.Id == news.Id);

            if (target == null)
            {
                return Content("{\"isSucceed\":false,\"msg\":\"您要編輯的公告不存在\"}", "application/json");
            }

            target.Type = news.Type;
            target.Title = news.Title;
            target.Content = news.Content;
            _db.SaveChanges();

            return Content("{\"isSucceed\":true}", "application/json");
        }

        public ActionResult DeleteAnnouncement(int id)
        {
            if (Session["admin"] == null)
            {
                return new HttpStatusCodeResult(401);
            }

            var target = _db.News.FirstOrDefault(x => x.Id == id);

            if (target == null)
            {
                return Content("{\"isSucceed\":false,\"msg\":\"您要刪除的公告不存在\"}", "application/json");
            }

            _db.News.Remove(target);
            _db.SaveChanges();

            return Content("{\"isSucceed\":true}", "application/json");
        }

        public ActionResult CreateAnnouncement(Models.News news)
        {
            if (Session["admin"] == null)
            {
                return new HttpStatusCodeResult(401);
            }

            if (news.Title == "" || news.Content == "")
            {
                return Content("{\"isSucceed\":false,\"msg\":\"公告資料不完整\"}", "application/json");
            }

            news.CreatedDate = DateTime.Now;
            _db.News.Add(news);
            _db.SaveChanges();

            return Content("{\"isSucceed\":true}", "application/json");
        }

    }
}