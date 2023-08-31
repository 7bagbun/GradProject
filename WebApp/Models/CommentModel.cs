using System;

namespace WebApp.Models
{
    public class CommentModel
    {
        public string Author { get; set; }
        public int Rating { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}