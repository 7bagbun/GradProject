using System;
using System.Linq;
using System.Web.Mvc;
using WebApp.Models;
using WebApp.Models.ViewModels;

namespace WebApp.Controllers.Account
{
    public class AccountController : Controller
    {
        private readonly TestDbEntities _db = new TestDbEntities();

        public ActionResult Login(string username, string passwd)
        {
            try
            {
                var target = _db.Member.FirstOrDefault(
                    m => m.Username == username && m.Password == passwd);

                if (target == null)
                {
                    return Json(new { result = false, msg = "帳號或密碼錯誤" });
                }
                else if (!target.Verified)
                {
                    return Json(new { result = false, msg = "請先完成信箱驗證後再登入" });
                }
                else if (target.Suspended)
                {
                    return Json(new { result = false, msg = "此帳號已被停權" });
                }

                Session["user"] = target.Username;
                Session["userId"] = target.Id;

                if (target.IsAdmin)
                {
                    Session["admin"] = true;
                    return Json(new { result = true, msg = "登入成功", admin = true, redirUrl = "/admin/dashboard" });
                }

                return Json(new { result = true, msg = "登入成功" });
            }
            catch (Exception)
            {
                return new HttpStatusCodeResult(500);
            }
        }
        public ActionResult Logout()
        {
            Session["userId"] = null;
            Session["admin"] = null;
            Session["user"] = null;
            return RedirectToAction("index", "home");
        }

        [HttpPost]
        public ActionResult ChangePassword(string oldPwd, string newPwd)
        {
            if (Session["userId"] == null)
            {
                return Content("{\"isSucceed\":false,\"msg\":\"使用者未登入。\"}", "application/json");
            }

            int id = (int)Session["userId"];
            var target = _db.Member.FirstOrDefault(x => x.Id == id);

            if (target.Password != oldPwd)
            {
                return Content("{\"isSucceed\":false,\"msg\":\"輸入的舊密碼錯誤。\"}", "application/json");
            }

            target.Password = newPwd;
            _db.SaveChanges();

            return Content("{\"isSucceed\":true,\"msg\":\"修改成功！\"}", "application/json");
        }

        public ActionResult ShowProfile(string tab)
        {
            if (Session["userId"] == null)
            {
                return RedirectToAction("NoLogin", "Redirect");
            }

            Models.Member target;
            int id = (int)Session["userId"];

            switch (tab)
            {
                case "track":
                    ViewBag.Tab = tab;
                    target = _db.Member.Include("TrackProduct").FirstOrDefault(x => x.Id == id);
                    break;
                case "comment":
                    ViewBag.Tab = tab;
                    target = _db.Member.Include("Comment").FirstOrDefault(x => x.Id == id);
                    break;
                case "profile":
                    ViewBag.Tab = tab;
                    target = _db.Member.FirstOrDefault(x => x.Id == id);
                    break;
                default:
                    ViewBag.Tab = "track";
                    target = _db.Member.Include("TrackProduct").FirstOrDefault(x => x.Id == id);
                    break;
            }

            return View(target);
        }

        public ActionResult ProfileTrackProduct()
        {
            if (Session["userId"] == null)
            {
                return Content("{\"isSucceed\":false}");
            }

            int id = (int)Session["userId"];
            var target = _db.Member.Include("TrackProduct").FirstOrDefault(x => x.Id == id);

            return View(target);
        }

        public ActionResult ProfileComment()
        {
            if (Session["userId"] == null)
            {
                return Content("{\"isSucceed\":false}");
            }

            int id = (int)Session["userId"];
            var target = _db.Member.Include("Comment").FirstOrDefault(x => x.Id == id);

            return View(target);
        }

        public ActionResult ProfilePersonalData()
        {
            if (Session["userId"] == null)
            {
                return Content("{\"isSucceed\":false}");
            }

            int id = (int)Session["userId"];
            var target = _db.Member.FirstOrDefault(x => x.Id == id);

            return View(target);
        }
    }
}