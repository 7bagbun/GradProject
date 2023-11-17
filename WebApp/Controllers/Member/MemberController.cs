using WebApp.Models;
using System;
using System.Linq;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Web;
using System.Net.Mail;
using WebApp.Misc;
using System.Net;

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
                }
                else if (member.Password == "")
                {
                    msg = $"無效的密碼，請嘗試其他密碼。";
                }
                else if (member.Password.Length < 6 || member.Password.Length > 20)
                {
                    msg = $"已超過密碼長度限制(6-20個字元)，請嘗試其他密碼。";
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

                SendVerificationEmail(member);

                member.CreatedDate = DateTime.Now;
                _db.Member.Add(member);
                _db.SaveChanges();

                return Content($"{{\"isSucceed\":true, \"redirUrl\":\"/member/hintVerifyEmail?email={member.Email}\"}}", "application/json");
            }
            catch (Exception)
            {
                return new HttpStatusCodeResult(500);
            }
        }

        private void SendVerificationEmail(Models.Member member)
        {
            string code = member.Username;
            var sm = new SecretManager(Server.MapPath("~"));
            string key = sm.GetSecret("aesKey");
            string iv = new Random().Next().ToString();

            code = AesHelper.AesEncrypt(key, iv, code);

            string serverUrl = $"https://{Request.Url.Authority}/member/verifymember?code={code}&iv={iv}";
            string subject = "愛家電會員信箱驗證信";
            string content = System.IO.File.ReadAllText(Server.MapPath("~/Assets/EmailTemplates/VerifyAccount.html"));
            content = content.Replace("${url}", serverUrl);

            MailMessage mms = new MailMessage
            {
                From = new MailAddress("iiihomeappliances@gmail.com"),
                Subject = subject,
                Body = content,
                IsBodyHtml = true,
                SubjectEncoding = System.Text.Encoding.UTF8
            };

            new EmailHelper(Server.MapPath("~")).SendEmail(mms, new MailAddress(member.Email, null, System.Text.Encoding.UTF8));
        }

        public ActionResult HintVerifyEmail(string email)
        {
            ViewBag.Email = email;
            return View();
        }

        public ActionResult VerifyMember(string code, string iv)
        {
            try
            {
                var sm = new SecretManager(Server.MapPath("~"));
                string key = sm.GetSecret("aesKey");
                code = AesHelper.AesDecrypt(key, iv, code);

                var target = _db.Member.FirstOrDefault(x => x.Username == code);
                target.Verified = true;
                _db.SaveChanges();

                Session["userId"] = target.Id;
                Session["user"] = target.Username;

                return RedirectToAction("Succeed", "Redirect", new { msg = "已成功驗證信箱，" });
            }
            catch (Exception)
            {
                return RedirectToAction("Error", "Redirect", new { msg = "驗證失敗，" });
            }
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

        public ActionResult EditPfp(HttpPostedFileBase pfp)
        {
            if (Session["userId"] == null)
            {
                return Content("{\"isSucceed\":false}");
            }

            int id = (int)Session["userId"];
            var user = _db.Member.Include("Image").FirstOrDefault(x => x.Id == id);

            if (user == null)
            {
                return Content("{\"isSucceed\":false}");
            }

            var imageBytes = new byte[pfp.InputStream.Length];
            pfp.InputStream.Read(imageBytes, 0, imageBytes.Length);
            user.Image.ImageContent = imageBytes;
            _db.SaveChanges();

            return Content("{\"isSucceed\":true}");
        }
    }
}