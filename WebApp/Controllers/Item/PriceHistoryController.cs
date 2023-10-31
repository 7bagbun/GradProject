using Newtonsoft.Json;
using System.Linq;
using System.Web.Mvc;
using WebApp.Models;
using System.Collections;
using System;
using Microsoft.Ajax.Utilities;
using System.Collections.Generic;

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
            });

            var config = new JsonSerializerSettings() { DateFormatString = "yyyy/MM/dd hh:mm:ss" };
            string json = JsonConvert.SerializeObject(diff, config);

            return Content(json, "application/json");
        }
    }
}