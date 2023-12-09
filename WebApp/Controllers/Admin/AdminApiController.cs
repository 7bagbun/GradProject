using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApp.Models;

namespace WebApp.Controllers.Admin
{
    public class AdminApiController : Controller
    {
        private readonly TestDbEntities _dbEntities = new TestDbEntities();

        public ActionResult ReportComment()
        {
            var report = _dbEntities.ReportComment.Include("Member").Include("Comment").Select(
                x => new
                {
                    id = x.Id,
                    reportMember = x.Member.Username,
                    author = x.Comment1.Member.Username,
                    comment = x.Comment1.Content,
                    status = x.Status,
                    reason = x.ReportReason,
                    reportTime = x.ReportTime
                });

            string json = JsonConvert.SerializeObject(report, new JsonSerializerSettings() { DateFormatString = "yyyy/MM/dd"});

            return Content(json, "application/json");
        }
    }
}