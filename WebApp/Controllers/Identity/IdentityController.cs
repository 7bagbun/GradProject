using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebApp.Controllers.Identity
{
    public class IdentityController : Controller
    {
        public ActionResult GetIdentity()
        {
            string identity = "none";

            if (Session["userId"] != null)
            {
                identity = "member";
            }

            return Content($"{{\"identity\":\"{identity}\"}}", "application/json");
        }
    }
}