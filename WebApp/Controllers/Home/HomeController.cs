using WebApp.Models;
using System.Linq;
using System.Web.Mvc;
using System.Security.Permissions;
using Microsoft.Ajax.Utilities;

namespace WebApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly TestDbEntities _db = new TestDbEntities();
        private readonly int _displayAmount = 8; //amount of items displayed in home page

        public ActionResult Index()
        {
            var sellings = new Selling[_displayAmount];
            var prods = _db.Product.Include("Selling")
                .OrderByDescending(x => x.Views).Take(_displayAmount).ToArray();

            for (int i = 0; i < _displayAmount; i++)
            {
                sellings[i] = prods[i].Selling.First();
            }

            return View(sellings);
        }
    }
}