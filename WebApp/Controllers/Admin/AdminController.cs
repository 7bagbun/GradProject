using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApp.Models;

namespace WebApp.Controllers.Admin
{
    public class AdminController : Controller
    {
        private readonly TestDbEntities _db = new TestDbEntities();

        public ActionResult Dashboard()
        {
            var vm = new DashboardViewModel
            {
                Comments = _db.Comment.Include("Member").OrderByDescending(x => x.CreatedDate).Take(10).ToArray()
            };

            ViewBag.MemberCount = _db.Member.Where(
                x => x.Verified == true && x.Suspended == false && x.IsAdmin == false).Count();

            ViewBag.CommentCount = _db.Comment.Count();

            return View(vm);
        }

        public ActionResult MemberList()
        {
            var members = _db.Member.Where(x => x.IsAdmin == false).ToArray();

            return View(members);
        }

        public ActionResult EditMember(int id)
        {
            var target = _db.Member.FirstOrDefault(x => x.Id == id);

            if (target == null)
            {
                return RedirectToAction("MemberList");
            }

            var vm = new MemberViewModel
            {
                Member = target,
                Comments = _db.Comment.Where(x => x.Author == id).OrderByDescending(x => x.CreatedDate).Take(10).ToArray(),
                TrackProducts = _db.TrackProduct.Where(x => x.Follower == id).OrderByDescending(x => x.FollowTime).Take(10).ToArray(),
            };

            return View(vm);
        }

        public ActionResult AnnouncementList()
        {
            var news = _db.News.OrderByDescending(x => x.CreatedDate).ToArray();

            return View(news);
        }

        public ActionResult EditAnnouncement(int id)
        {
            var news = _db.News.FirstOrDefault(x => x.Id == id);

            return View(news);
        }

        public ActionResult CreateAnnouncement()
        {
            var news = new Models.News
            {
                CreatedDate = DateTime.Now,
            };

            return View(news);
        }

        public ActionResult CommentList()
        {
            var comments = _db.Comment.OrderByDescending(x => x.CreatedDate).ToArray();

            return View(comments);
        }
        
        public ActionResult CommentDetail(int id)
        {
            var target = _db.Comment.FirstOrDefault(x => x.Id == id);

            return View(target);
        }

    }
}