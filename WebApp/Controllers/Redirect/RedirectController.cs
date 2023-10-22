using System.Web.Mvc;

namespace WebApp.Controllers.Redirect
{
    public class RedirectController : Controller
    {
        public ActionResult NoLogin()
        {
            return View();
        }

        public ActionResult Error(string msg)
        {
            ViewBag.Message = msg;
            return View();
        }

        public ActionResult Inform(string msg)
        {
            ViewBag.Message = msg;
            return View();
        }
        public ActionResult Succeed(string msg)
        {
            ViewBag.Message = msg;
            return View();
        }
    }
}