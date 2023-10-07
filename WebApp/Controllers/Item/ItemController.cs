using WebApp.Models;
using System.Linq;
using System.Web.Mvc;
using Newtonsoft.Json;
using System.Web.UI.WebControls;
using Microsoft.Ajax.Utilities;

namespace WebApp.Controllers.Item
{
    public class ItemController : Controller
    {
        private readonly TestDbEntities _db = new TestDbEntities();

        public ActionResult List(int id)
        {
            var items = _db.Selling.Include("Source1")
                            .Where(x => x.Product == id).OrderBy(x => x.Price);

            if (items.Count() == 0) return HttpNotFound();

            int modelId = items.First().Product;
            var model = _db.Product.FirstOrDefault(x => x.Id == modelId);

            ViewBag.Model = model.Model;
            ViewBag.Brand = model.Brand;
            ViewBag.Type = model.ProductType;
            ViewBag.ProductId = modelId;

            return View(items);
        }

        public ActionResult Get(int typeCount, int itemCount)
        {
            var list = new SellingProductList[typeCount];

            for (int i = 0; i < typeCount; i++)
            {
                list[i] = new SellingProductList();
                list[i].Model = _db.Product.First(x => x.Id == i + 1).Model;

                string modelNum = list[i].Model;
                list[i].Items = _db.Selling.Include("Product1").Where(x => x.Product1.Model == modelNum)
                                    .Take(itemCount).ToArray();
            }

            var dateSettings = new JsonSerializerSettings() { DateFormatString = "yyyy/MM/dd HH:mm:ss" };
            string json = JsonConvert.SerializeObject(list, dateSettings);
            return Content(json, "application/json");
        }
        
        public ActionResult Search(string query)
        {
            if (query == null)
            {
                var list = _db.Product.Include("Selling").Take(10);
                return View(list);
            }

            var result = _db.Product.Include("Selling")
                .Where(x =>
                    x.Model.Contains(query) ||
                    x.Brand.Contains(query) ||
                    x.ProductType.Contains(query) ||
                    x.Token.Contains(query));

            //order by price
            result.ForEach(x => x.Selling = x.Selling.OrderByDescending(t => t.Price).ToList());

            return View(result);
        }

    }
}