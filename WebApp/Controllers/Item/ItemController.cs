using WebApp.Models;
using System.Linq;
using System.Web.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace WebApp.Controllers.Item
{
    public class ItemController : Controller
    {
        private readonly TestDbEntities _db = new TestDbEntities();

        public ActionResult List(int id, string tab)
        {
            var product = _db.Product.Include("PriceHistory").Include("Selling").FirstOrDefault(x => x.Id == id);
            if (product == null)
            {
                return HttpNotFound();
            }

            switch (tab)
            {
                case "list":
                    tab = "list";
                    break;
                case "comment":
                    tab = "comment";
                    break;
                default:
                    tab = "list";
                    break;
            }

            ViewBag.ProductId = product.Id;
            ViewBag.UpdatedTime = product.PriceHistory.OrderByDescending(x => x.UpdatedTime).FirstOrDefault(x => x.Product == id).UpdatedTime;
            ViewBag.Image = product.Selling.FirstOrDefault(x => x.Product == id)?.Image ?? 0;
            ViewBag.Model = product.Model;
            ViewBag.Brand = product.Brand;
            ViewBag.Type = product.ProductType;
            ViewBag.Tab = tab;

            product.Views++;
            _db.SaveChanges();

            return View();
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
                    source = x.Source1.DisplayName,
                    price = x.Price,
                    fprice = x.Price.ToString("C0"),
                    url = x.Source1.Domain + x.Link,
                    sourceImage = "/Assets/Images/" + x.Source1.ImageName,
                    image = "/image/get/" + x.Image
                });

            string json = JsonConvert.SerializeObject(obj);
            return Content(json, "application/json");
        }

        public ActionResult GetTypes()
        {
            var types = _db.ProductType.Select(
                x => new
                {
                    id = x.Id,
                    type = x.Type
                }).ToArray();

            string json = JsonConvert.SerializeObject(types);
            return Content(json, "application/json");
        }

        public ActionResult SearchPage(string query = "", int cateId = 1)
        {
            ViewBag.Query = query;
            ViewBag.CateId = cateId;
            return View();
        }

        public ActionResult Search(string query, int cateId = 0, int page = 1)
        {
            query = query ?? "";
            query = query.Trim();
            var multiQuery = query.Split(' ');
            var result = Enumerable.Empty<Product>();

            page = --page < 1 ? 1 : page * 20;


            if (cateId > 1)
            {
                result = _db.Product.Where(x => x.Selling.FirstOrDefault() != null && x.Type == cateId);
            }
            else
            {
                result = _db.Product.Where(x => x.Selling.FirstOrDefault() != null);
            }

            foreach (var item in multiQuery)
            {
                result = result.Where(x =>
                                   x.Model.Contains(item) ||
                                   x.Brand.Contains(item) ||
                                   x.ProductType.Contains(item) ||
                                   (x.Token?.Contains(item) ?? false));
            }

            int totalPages = (int)Math.Ceiling((double)result.Count() / 20);
            totalPages = totalPages < 1 ? 1 : totalPages;
            result = result.Skip(page - 1).Take(20);

            var obj = result.Select(x => new
            {
                id = x.Id,
                brand = x.Brand,
                model = x.Model,
                type = x.ProductType,
                image = x.Selling.FirstOrDefault().Image,
                fprice = x.Selling.FirstOrDefault().Price.ToString("C0"),
                price = x.Selling.FirstOrDefault().Price,
                popularity = x.Views
            });

            string json = JsonConvert.SerializeObject(new { totalPages, products = obj });

            return Content(json, "application/json");
        }
    }
}