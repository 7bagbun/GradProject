using System.Linq;
using System.Web.Mvc;
using WebApp.Models;

namespace WebApp.Controllers.Admin
{
    public class ModerationController : Controller
    {
        private readonly TestDbEntities _db = new TestDbEntities();

        public ActionResult DeleteComment()
        {
            return View();
        }

        [HttpPost]
        public ActionResult SetSuspend(int id, bool state)
        {
            if (Session["admin"] == null)
            {
                return new HttpStatusCodeResult(401);
            }

            var target = _db.Member.FirstOrDefault(x => x.Id == id);

            if (target == null)
            {
                return Content("{\"isSuceed\":false,\"msg\":\"無此會員\"}");
            }

            target.Suspended = state;
            _db.SaveChanges();

            return Content("{\"isSucceed\":true}", "application/json");
        }

        [HttpPost]
        public ActionResult DeleteComment(int commentId)
        {
            if (Session["admin"] == null)
            {
                return new HttpStatusCodeResult(401);
            }

            var comment = _db.Comment.FirstOrDefault(x => x.Id == commentId);

            if (comment == null)
            {
                return Content("{\"isSucceed\":false,\"msg\":\"您要刪除的評論不存在\"}", "application/json");
            }

            _db.Comment.Remove(comment);
            _db.SaveChanges();

            return Content("{\"isSucceed\":true}", "application/json");
        }

    }
}