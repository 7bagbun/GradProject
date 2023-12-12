using Newtonsoft.Json;
using System.Linq;
using System.Web.Mvc;
using WebApp.Models;
using Microsoft.Ajax.Utilities;
using System.Collections.Generic;
using WebApp.Misc;
using System.Net.Mail;

namespace WebApp.Controllers.Item
{
    public class PriceHistoryController : Controller
    {
        private readonly TestDbEntities _db = new TestDbEntities();

        public ActionResult Get(int productId)
        {
            var history = _db.PriceHistory.Where(x => x.Product == productId)
                .Select(x => new { x.UpdatedTime, x.Price })
                .OrderBy(x => x.UpdatedTime);

            var diff = new List<PriceHistoryModel>(4);

            int current = 0;
            int index = 1;
            int length = history.Count();
            history.ForEach(x =>
            {
                if (x.Price != current || index == length)
                {
                    current = x.Price;
                    diff.Add(new PriceHistoryModel
                    {
                        UpdatedTime = x.UpdatedTime,
                        Price = x.Price
                    });
                }

                index++;
            });

            var config = new JsonSerializerSettings() { DateFormatString = "yyyy/MM/dd hh:mm:ss" };
            string json = JsonConvert.SerializeObject(diff, config);

            return Content(json, "application/json");
        }

        public ActionResult CheckPrice()
        {
            try
            {
                var prods = _db.Product.Include("TrackProduct").Where(x => x.CurrentLow < x.PreviousLow);

                if (prods.Any())
                {
                    prods.ForEach(x =>
                    {
                        x.PreviousLow = x.CurrentLow;
                    });

                    SendEmailNotification(prods);

                    _db.SaveChanges();
                }

                return Content("{\"msg\":\"執行成功\"}", "application/json");
            }
            catch (System.Exception ex)
            {
                return Content($"{{\"msg\":\"執行失敗：{ex.Message}\"}}", "application/json");
            }
        }

        private void SendEmailNotification(IEnumerable<Product> prods)
        {
            var eh = new EmailHelper(Server.MapPath("~"));

            string serverUrl = $"https://{Request.Url.Authority}/";
            string subject = "愛家電產品追蹤通知信";
            string template = System.IO.File.ReadAllText(Server.MapPath("~/Assets/EmailTemplates/ProductDiscount.html"));

            foreach (var item in prods)
            {
                var addrs = _db.TrackProduct
                               .Include("Follower")
                               .Where(x => x.Product == item.Id)
                               .Select(x => x.Member.Email)
                               .ToArray();

                if (!addrs.Any())
                {
                    continue;
                }

                string imgUrl = serverUrl + "image/get/" + _db.Selling.FirstOrDefault(x => x.Product == item.Id).Image;
                string itemUrl = serverUrl + "item/list/" + item.Id;
                string content = template;
                content = content.Replace("${pname}", $"{item.Brand} {item.Model} {item.ProductType}");
                content = content.Replace("${image}", imgUrl);
                content = content.Replace("${url}", itemUrl);

                using (var msg = new MailMessage())
                {
                    msg.From = new MailAddress("iiihomeappliances@gmail.com");
                    msg.Subject = subject;
                    msg.Body = content;
                    msg.IsBodyHtml = true;
                    msg.SubjectEncoding = System.Text.Encoding.UTF8;

                    eh.SendEmail(msg, addrs);
                }
            }
        }
    }
}