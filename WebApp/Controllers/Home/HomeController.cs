using WebApp.Models;
using System.Linq;
using System.Web.Mvc;
using System.Security.Permissions;
using Microsoft.Ajax.Utilities;

namespace WebApp.Controllers
{
    public class HomeController : Controller
    {
        //private readonly DbEntities _db = new DbEntities();
        private readonly TestDbEntities _db = new TestDbEntities();

        public ActionResult Index()
        {
            int amount = 8, count = 0;
            var selling = _db.Selling.Include("Product1").OrderBy(x => x.Price);
            var types = _db.Product.Take(amount).Select(x => x.Id);
            var prods = new Selling[amount];
            types.ForEach(i => prods[count++] = selling.First(x => x.Product1.Id == i));

            return View(prods);
        }
    }
}