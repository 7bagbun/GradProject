using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using WebApp.Misc;
using WebApp.Models;

namespace WebApp.Controllers.Member
{
    public class ForgetPasswordController : Controller
    {
        private readonly TestDbEntities _db = new TestDbEntities();

        public ActionResult ForgetPassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SendEmailCode(string email)
        {
            var target = _db.Member.FirstOrDefault(x => x.Email == email);

            if (target == null)
            {
                return Content("{\"isSucceed\":false,\"msg\":\"輸入的會員名稱不存在。\"}", "application/json");
            }

            string code = $"{target.Id}|{target.Username}|{DateTime.Now}";
            var sm = new SecretManager(Server.MapPath("~"));
            string key = sm.GetSecret("aesKey");
            string iv = new Random().Next().ToString();

            code = AesHelper.AesEncrypt(key, iv, code);

            string serverUrl = $"https://{Request.Url.Authority}/forgetpassword/verifycode";
            string subject = "愛家電重設密碼驗證信";
            string content = System.IO.File.ReadAllText(Server.MapPath("~/Assets/EmailTemplates/ResetPassword.html"));
            content = content.Replace("${url}", $"{serverUrl}?code={code}&iv={iv}");

            MailMessage mms = new MailMessage
            {
                From = new MailAddress("iiihomeappliances@gmail.com"),
                Subject = subject,
                Body = content,
                IsBodyHtml = true,
                SubjectEncoding = System.Text.Encoding.UTF8
            };

            new EmailHelper(Server.MapPath("~")).SendEmail(mms, new MailAddress(target.Email, null, System.Text.Encoding.UTF8));

            return Content("{\"isSucceed\":true,\"msg\":\"已發送驗證信，請於15分鐘內至該信箱驗證。\"}", "application/json");
        }

        public ActionResult VerifyCode(string code, string iv)
        {
            try
            {
                var sm = new SecretManager(Server.MapPath("~"));
                string key = sm.GetSecret("aesKey");
                code = AesHelper.AesDecrypt(key, iv, code);
                string[] data = code.Split('|');

                var resetTime = Convert.ToDateTime(data[2]);
                var ts = new TimeSpan(DateTime.Now.Ticks - resetTime.Ticks);
                if (ts.TotalMinutes >= 15)
                {
                    return RedirectToAction("Error", "Redirect", new { msg = "已超過驗證碼有效時間，請重新發送驗證碼。" });
                }

                Session["userId"] = int.Parse(data[0].ToString());
                Session["user"] = data[1];
                Session["resetTime"] = DateTime.Now;
                return RedirectToAction("ResetPassword");
            }
            catch (Exception)
            {
                throw;
            }
        }

        public ActionResult ResetPassword()
        {
            if (Session["userId"] == null)
            {
                return RedirectToAction("NoLogin", "Redirect");
            }
            else if (Session["resetTime"] == null)
            {
                return RedirectToAction("Error", "Redirect", new { msg = "認證錯誤。" });
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ResetPassword(string newPassword)
        {
            if (Session["userId"] == null)
            {
                return RedirectToAction("NoLogin", "Redirect");
            }
            else if (Session["resetTime"] == null)
            {
                return RedirectToAction("Error", "Redirect", new { msg = "認證錯誤。" });
            }
            else if (newPassword == "")
            {
                ViewBag.Message = "請輸入新密碼。";
                return View();
            }

            int id = (int)Session["userId"];
            var target = _db.Member.FirstOrDefault(x => x.Id == id);

            if (target.Password == newPassword)
            {
                ViewBag.Message = "新密碼與舊密碼不可相同。";
                return View();
            }

            target.Password = newPassword;
            _db.SaveChanges();
            Session["resetTime"] = null;

            return RedirectToAction("Succeed", "Redirect", new { msg = "已成功更改密碼，" });
        }
    }
}