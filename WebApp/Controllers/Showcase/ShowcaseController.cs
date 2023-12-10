using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using WebApp.Models;

namespace WebApp.Controllers.Showcase
{
    public class ShowcaseController : Controller
    {
        private readonly TestDbEntities _db = new TestDbEntities();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult StartRunning()
        {
            try
            {
                string path = Server.MapPath("~/scraper/Scraper.exe");
                ProcessStartInfo info = new ProcessStartInfo
                {
                    FileName = path,
                    Arguments = "-S",
                    UseShellExecute = false,
                    RedirectStandardError = true,
                };

                var proc = Process.Start(info);
                proc.WaitForExit();
                string msg = proc.StandardError.ReadToEnd();

                if (proc.ExitCode != 0)
                {
                    string json = JsonConvert.SerializeObject(new { msg });
                    return Content(json, "application/json");
                }

                return Content("{\"msg\":\"執行成功\"}", "application/json");
            }
            catch (Exception ex)
            {
                return Content($"{{\"msg\":\"執行失敗：{ex.Message}\"}}", "application/json");
            }
        }

        public ActionResult FakeEmail()
        {
            var prods = _db.Product.Take(10).ToArray();

            foreach (var item in prods)
            {
                item.CurrentLow = item.PreviousLow - 100;
            }

            _db.SaveChanges();

            return RedirectToAction("CheckPrice", "PriceHistory");
        }
    }
}