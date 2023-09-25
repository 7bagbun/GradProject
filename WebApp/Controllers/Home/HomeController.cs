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
        private readonly int _displayAmount = 8; //amount of items displayed in home page

        public ActionResult Index()
        {
            int count = 0;
            var selling = _db.Selling.Include("Product1").OrderBy(x => x.Price);
            var types = _db.Product.Take(_displayAmount).Select(x => x.Id);
            var prods = new Selling[_displayAmount];
            types.ForEach(i => prods[count++] = selling.First(x => x.Product1.Id == i));

            return View(prods);
        }
    }
}