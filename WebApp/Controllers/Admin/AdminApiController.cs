using Newtonsoft.Json;
using System.Linq;
using System.Web.Mvc;
using WebApp.Models;

namespace WebApp.Controllers.Admin
{
    public class AdminApiController : Controller
    {
        private readonly TestDbEntities _db = new TestDbEntities();

        public ActionResult ReportComment()
        {
            var report = _db.ReportComment.Include("Member").Include("Comment").Select(
                x => new
                {
                    id = x.Id,
                    reportMember = x.Member.Username,
                    author = x.Comment1.Member.Username,
                    commentId = x.Comment,
                    comment = x.Comment1.Content,
                    status = x.Status,
                    reason = x.ReportReason,
                    reportTime = x.ReportTime
                });

            string json = JsonConvert.SerializeObject(report, new JsonSerializerSettings() { DateFormatString = "yyyy/MM/dd"});

            return Content(json, "application/json");
        }

        public ActionResult GetProduct()
        {
            var prods = _db.Product.Include("ProductType1").Select(
                x => new
                {
                    id = x.Id,
                    pType = x.ProductType,
                    type = x.Type,
                    typeName = x.ProductType1.Type,
                    brand = x.Brand,
                    model = x.Model,
                    token = x.Token,
                    views = x.Views,
                });

            string json = JsonConvert.SerializeObject(prods);

            return Content(json, "application/json");
        }
    }
}