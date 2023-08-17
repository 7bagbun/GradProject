using WebApp.Models;
using System;
using System.Linq;
using System.Web.Mvc;

namespace WebApp.Controllers.Member
{
    public class LoginController : Controller
    {
        private readonly TestDbEntities _db = new TestDbEntities();

        public ActionResult Register()
        {
            return View(new Models.Member() { CreatedDate = DateTime.Now });
        }

        [HttpPost]
        public ActionResult Register(Models.Member member)
        {
            try
            {
                var existed = _db.Member.FirstOrDefault(m => m.Username == member.Username);

                if (existed != null)
                {
                    ViewBag.Message = $"Username \"{member.Username}\" has already existed!";
                    return View(member);
                }

                Session["user"] = member.Username;
                member.CreatedDate = DateTime.Now;
                _db.Member.Add(member);
                _db.SaveChanges();
                return RedirectToAction("index", "home");

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }

        [HttpPost]
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
                else
                {
                    Session["user"] = target.Username;
                    return Json(new { result = true, msg = "登入成功" });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }

        public ActionResult Logout()
        {
            Session["user"] = null;
            return RedirectToAction("index", "home");
        }
    }
}