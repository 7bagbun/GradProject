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

        [HttpPost]
        public ActionResult TrackAjax(int productId)
        {
            string json;

            if (Session["userId"] == null)
            {
                json = "{\"isSucceed\":false,\"redirUrl\":\"/error/nologin\"}";
                return Content(json, "application/json");
            }

            if (!_db.Product.Any(x => x.Id == productId))
            {
                json = "{\"isSucceed\":false,\"redirUrl\":\"/error/custommessage?msg=無此商品\"}";
                return Content(json, "application/json");
            }

            int mid = (int)Session["userId"];

            var followed = _db.TrackProduct.FirstOrDefault(
                x => x.Product == productId && x.Follower == mid);

            if (followed != null) //if already followed then unfollow
            {
                _db.TrackProduct.Remove(followed);
                _db.SaveChanges();
                json = "{\"isSucceed\":true}";
                return Content(json, "application/json");
            }

            _db.TrackProduct.Add(new TrackProduct { Product = productId, Follower = mid });
            _db.SaveChanges();

            json = "{\"isSucceed\":true}";
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

        public ActionResult TrackStatusAjax(int productId)
        {
            if (Session["userId"] == null)
            {
                return Content("{\"isTracked\":false}", "application/json");
            }

            int mid = (int)Session["userId"];
            string isTracked = _db.TrackProduct.Any(x =>
                x.Product == productId && x.Follower == mid) ? "true" : "false";

            return Content($"{{\"isTracked\":{isTracked}}}", "application/json");
        }
    }
}