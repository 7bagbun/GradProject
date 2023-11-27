using WebApp.Models;
using System.Linq;
using System.Web.Mvc;
using Newtonsoft.Json;
using System.Web.UI.WebControls;
using Microsoft.Ajax.Utilities;
using WebApp.Models.ViewModels;

namespace WebApp.Controllers.Item
{
    public class ItemController : Controller
    {
        private readonly TestDbEntities _db = new TestDbEntities();

        public ActionResult List(int id, string tab, string sort)
        {
            var vm = new ItemListViewModel();

            if (Session["userId"] != null)
            {
                int userId = (int)Session["userId"];
                vm.Comment = _db.Comment.FirstOrDefault(x => x.Product == id && x.Author == userId);
            }

            vm.Items = _db.Selling.Include("Source1")
                            .Where(x => x.Product == id);
            if (vm.Items.FirstOrDefault() == null) return HttpNotFound();

            switch (sort)
            {
                case "p":
                    vm.Items = vm.Items.OrderBy(x => x.Price);
                    break;
                case "pr":
                    vm.Items = vm.Items.OrderByDescending(x => x.Price);
                    break;
                case "s":
                    vm.Items = vm.Items.OrderBy(x => x.Source);
                    break;
                case "sr":
                    vm.Items = vm.Items.OrderByDescending(x => x.Source);
                    break;
                default:
                    vm.Items = vm.Items.OrderBy(x => x.Price);
                    break;
            }


            int modelId = vm.Items.First().Product;
            var model = _db.Product.FirstOrDefault(x => x.Id == modelId);

            ViewBag.UpdatedTime = vm.Items.First().UpdatedTime;
            ViewBag.Model = model.Model;
            ViewBag.Brand = model.Brand;
            ViewBag.Type = model.ProductType;
            ViewBag.ProductId = modelId;
            ViewBag.Tab = tab;

            _db.Product.FirstOrDefault(x => x.Id == id).Views++;
            _db.SaveChanges();

            return View(vm);
        }

        /*
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
        */
        
        public ActionResult Get(int id)
        {
            var prods = _db.Selling.Where(x => x.Product == id).OrderBy(x => x.Price).ToArray();
            var obj = prods.Select(
                x => new
                {
                    title = x.Title,
                    source = x.Source1.SourceName,
                    price = x.Price,
                    fprice = x.Price.ToString("C0"),
                    url = x.Source1.Domain + x.Link,
                    image = "/image/get/" + x.Image
                });

            string json = JsonConvert.SerializeObject(obj);
            return Content(json, "application/json");
        }

        public ActionResult Search(string query)
        {
            if (query == null)
            {
                var list = _db.Product.Include("Selling").Take(12);
                return View(list.ToList());
            }

            var result = _db.Product.Include("Selling")
                .Where(x =>
                    x.Model.Contains(query) ||
                    x.Brand.Contains(query) ||
                    x.ProductType.Contains(query) ||
                    x.Token.Contains(query));

            //order by price
            result.ForEach(x => x.Selling = x.Selling.OrderByDescending(t => t.Price).ToList());

            return View(result.ToList());
        }
    }
}