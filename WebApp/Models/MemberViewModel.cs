using System.Collections.Generic;

namespace WebApp.Models
{
    public class MemberViewModel
    {
        public Member Member { get; set; }
        public int ValidReports { get; set; }
        public int FalseReports { get; set; }
        public int Violations { get; set; }
        public LoginRecord LoginRecord { get; set; }
        public IEnumerable<TrackProduct> TrackProducts { get; set; }
        public IEnumerable<Comment> Comments { get; set; }
    }
}