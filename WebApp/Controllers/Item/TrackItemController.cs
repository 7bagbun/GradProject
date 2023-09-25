using Microsoft.Ajax.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApp.Models;

namespace WebApp.Controllers.Item
{
    public class TrackItemController : Controller
    {
        private readonly TestDbEntities _db = new TestDbEntities();

        public ActionResult TrackItemAjax(int productId, int memberId)
        {
            if (!_db.Product.Any(x => x.Id == productId)
                || !_db.Member.Any(x => x.Id == memberId))
            {
                return Content("無此會員或商品");
            }

            string json = "{\"IsSucceed\":true}";
            return Content(json, "application/json");
        }

        public ActionResult TrackList()
        {
            if (Session["userId"] == null)
            {
                return RedirectToAction("nologin", "error");
            }

            int memberId = (int)Session["userId"];
            var followList = _db.TrackProduct.Include("Product1").Where(x => x.Follower == memberId);

            return View(followList);
        }
    }
}