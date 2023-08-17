using WebApp.Models;
using System;
using System.Linq;
using System.Web.Mvc;

namespace WebApp.Controllers.Member
{
    public class MemberController : Controller
    {
        private readonly TestDbEntities _db = new TestDbEntities();

        public ActionResult List()
        {
            try
            {
                var list = _db.Member.ToArray();
                return View(list);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }
    }
}