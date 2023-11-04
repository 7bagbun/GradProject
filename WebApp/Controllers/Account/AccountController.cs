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

                if (target.IsAdmin)
                {
                    Session["admin"] = true;
                }
                else
                {
                    Session["admin"] = false;
                }

                Session["user"] = target.Username;
                Session["userId"] = target.Id;
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
        
        public ActionResult ChangePassword()
        {
            if (Session["userId"] == null)
            {
                return RedirectToAction("NoLogin", "Redirect");
            }

            return View(new ChangePasswordViewModel());
        }

        [HttpPost]
        public ActionResult ChangePassword(ChangePasswordViewModel model)
        {
            if (Session["userId"] == null)
            {
                return RedirectToAction("NoLogin", "Redirect");
            }

            int id = (int)Session["userId"];
            var target = _db.Member.FirstOrDefault(x => x.Id == id);

            if (target.Password != model.OldPassword)
            {
                return RedirectToAction("CustomMessage", "輸入的舊密碼錯誤，請重新輸入。");
            }

            target.Password = model.NewPassword;
            _db.SaveChanges();
            return View(target);
        }
    }
}