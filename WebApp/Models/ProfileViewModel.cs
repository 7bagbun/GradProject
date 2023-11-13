using System.Collections.Generic;

namespace WebApp.Models
{
    public class ProfileViewModel
    {
        public Member Member { get; set; }
        public IEnumerable<TrackProduct> TrackProducts { get; set; }
        public IEnumerable<Comment> Comments { get; set; }
    }
}