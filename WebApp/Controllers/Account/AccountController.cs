﻿using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Remoting;
using System.Web.Mvc;
using WebApp.Models;

namespace WebApp.Controllers.Account
{
    public class AccountController : Controller
    {
        private readonly TestDbEntities _db = new TestDbEntities();

        public ActionResult LoginPage(string referer)
        {
            if (Session["userId"] != null)
            {
                return RedirectToAction("Index", "Home");
            }

            //prevent redirect to another domain
            if (referer.Contains("https://") || referer.Contains("http://"))
            {
                referer = "";
            }

            ViewBag.Referer = referer;
            return View();
        }

        public ActionResult Login(string username, string passwd, string referer)
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
                    RecordLogin(target.Id, Request.UserHostAddress);
                    return Json(new { result = true, msg = "登入成功", admin = true, redirUrl = "/admin/dashboard" });
                }

                RecordLogin(target.Id, Request.UserHostAddress);
                return Json(new { result = true, msg = "登入成功", referer });
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

        public ActionResult IsLogin()
        {
            if (Session["userId"] != null)
            {
                return Content("{\"isLogin\":true}", "application/json");
            }
            else
            {
                return Content("{\"isLogin\":false}", "application/json");
            }
        }

        private void RecordLogin(int id, string ip)
        {
            _db.LoginRecord.Add(new LoginRecord { Member = id, IP = ip, LoginTime = DateTime.Now });
            _db.SaveChanges();
        }
    }
}