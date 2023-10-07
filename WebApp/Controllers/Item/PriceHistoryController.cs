using Newtonsoft.Json;
using System.Linq;
using System.Web.Mvc;
using WebApp.Models;

namespace WebApp.Controllers.Item
{
    public class PriceHistoryController : Controller
    {
        private readonly TestDbEntities _db = new TestDbEntities();

        public ActionResult Get(int productId)
        {
            var history = _db.PriceHistory.Where(x => x.Product == productId)
                .Select(x => new { x.UpdatedTime, x.Price }).Distinct().OrderBy(x => x.UpdatedTime);

            var config = new JsonSerializerSettings() { DateFormatString = "yyyy/MM/dd hh:mm:ss" };
            string json = JsonConvert.SerializeObject(history, config);

            return Content(json, "application/json");
        }
    }
}