using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebApp.Controllers.Error
{
    public class ErrorController : Controller
    {
        public ActionResult NoLogin()
        {
            return View();
        }
    }
}