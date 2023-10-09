using System.Web.Mvc;

namespace WebApp.Controllers.Error
{
    public class ErrorController : Controller
    {
        public ActionResult NoLogin()
        {
            return View();
        }

        public ActionResult CustomMessage(string msg)
        {
            ViewBag.Message = msg;
            return View();
        }
    }
}