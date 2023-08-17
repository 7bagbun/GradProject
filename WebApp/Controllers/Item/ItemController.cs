using WebApp.Models;
using System.Linq;
using System.Web.Mvc;
using Newtonsoft.Json;

namespace WebApp.Controllers.Item
{
    public class ItemController : Controller
    {
        private readonly TestDbEntities _db = new TestDbEntities();
        // GET: Item
        public ActionResult List(int id)
        {
            var items = _db.Selling.Include("Source1")
                            .Where(x => x.Product == id).OrderBy(x => x.Price);

            int modelId = (int)items.First().Product;
            string model = _db.Product.FirstOrDefault(x => x.Id == modelId).Model;
            ViewBag.Model = model;

            return View(items);
        }

        public ActionResult All()
        {
            //var selling = _db.Selling.Include("Product1");
            //var prods = Enumerable.Range(2, 8).Select(i => selling.First(x => x.Product1.Id == i));

            return View();
        }

        public ActionResult Get(int typeCount, int itemCount)
        {
            var list = new ProductList[typeCount];

            for (int i = 0; i < typeCount; i++)
            {
                list[i] = new ProductList();
                list[i].Model = _db.Product.First(x => x.Id == i + 1).Model;

                string modelNum = list[i].Model;
                list[i].Items = _db.Selling.Include("Product1").Where(x => x.Product1.Model == modelNum)
                                    .Take(itemCount).ToArray();
            }

            var dateSettings = new JsonSerializerSettings() { DateFormatString = "yyyy/MM/dd HH:mm:ss" };
            string json = JsonConvert.SerializeObject(list, dateSettings);
            return Content(json, "application/json");
        }
    }
}