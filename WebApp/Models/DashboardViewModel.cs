using System.Collections.Generic;

namespace WebApp.Models
{
    public class DashboardViewModel
    {
        public IEnumerable<Comment> Comments { get; set; }
    }
}