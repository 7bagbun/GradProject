using WebApp.Models;
using System;
using System.Linq;
using System.Web.Mvc;
using WebApp.Models.ViewModels;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace WebApp.Controllers.Member
{
    public class MemberController : Controller
    {
        private readonly TestDbEntities _db = new TestDbEntities();

        public ActionResult List()
        {
            try
            {
                var list = _db.Member.ToArray();
                return View(list);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new HttpStatusCodeResult(500);
            }
        }

        public ActionResult Register()
        {
            return View(new Models.Member() { CreatedDate = DateTime.Now });
        }

        [HttpPost]
        public ActionResult Register(Models.Member member, HttpPostedFileBase pfp)
        {
            try
            {
                string msg = string.Empty;
                bool dupeUsername = _db.Member.Any(m => m.Username == member.Username);
                bool dupeEmail = _db.Member.Any(m => m.Email == member.Email);
                bool noNonWordChar = System.Text.RegularExpressions.Regex.IsMatch(member.Username, @"\W");

                if (noNonWordChar || member.Username.Length < 1)
                {
                    msg = $"無效的帳號名稱，請嘗試其他名稱。";
                }
                else if (member.Username.Length < 6 || member.Username.Length > 20)
                {
                    msg = $"已超過帳號長度限制(6-20個字元)，請嘗試其他名稱。";
                }
                else if (dupeUsername)
                {
                    msg = $"此帳號名稱已被使用，請嘗試其他名稱。";
                }
                else if (!new EmailAddressAttribute().IsValid(member.Email))
                {
                    msg = $"無效新的信箱地址，請重新輸入。";
                }
                else if (dupeEmail)
                {
                    msg = $"此信箱地址已被使用，請嘗試其他信箱。";
                }else if (member.Password == "")
                {
                    msg = $"無效的密碼，請嘗試其他密碼。";
                }

                if (msg != string.Empty)
                {
                    return Content($"{{\"isSucceed\":false, \"msg\":\"{msg}\"}}", "application/json");
                }

                if (pfp != null)
                {
                    byte[] imageBytes = new byte[pfp.ContentLength];
                    pfp.InputStream.Read(imageBytes, 0, imageBytes.Length);
                    member.Image = new Models.Image() { ImageContent = imageBytes };
                }

                Session["user"] = member.Username;
                member.CreatedDate = DateTime.Now;
                _db.Member.Add(member);
                _db.SaveChanges();

                msg = "註冊成功！已將您登入。";
                return Content($"{{\"isSucceed\":true, \"redirUrl\":\"/redirect/succeed?msg={msg}\"}}", "application/json");
            }
            catch (Exception)
            {
                return new HttpStatusCodeResult(500);
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
                    Session["userId"] = target.Id;
                    return Json(new { result = true, msg = "登入成功" });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new HttpStatusCodeResult(500);
                throw;
            }
        }

        public ActionResult Logout()
        {
            Session["user"] = null;
            Session["userId"] = null;
            return RedirectToAction("index", "home");
        }

        public ActionResult GetPfpById(int id)
        {
            var user = _db.Member.FirstOrDefault(x => x.Id == id);

            if (user == null)
            {
                return HttpNotFound();
            }
            else if (user.ProfilePicture == null)
            {
                var bytes = System.IO.File.ReadAllBytes(Server.MapPath("~/Assets/Images/default-pfp.png"));
                return File(bytes, "image/png");
            }

            var pfp = _db.Image.FirstOrDefault(x => x.Id == user.ProfilePicture);
            return File(pfp.ImageContent, "image/jpg");
        }

        public ActionResult GetPfpByUsername(string username)
        {
            var user = _db.Member.FirstOrDefault(x => x.Username == username);

            if (user == null)
            {
                return HttpNotFound();
            }
            else if (user.ProfilePicture == null)
            {
                var bytes = System.IO.File.ReadAllBytes(Server.MapPath("~/Assets/Images/default-pfp.png"));
                return File(bytes, "image/png");
            }

            var pfp = _db.Image.FirstOrDefault(x => x.Id == user.ProfilePicture);
            return File(pfp.ImageContent, "image/jpg");
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