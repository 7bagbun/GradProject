using System;

namespace WebApp.Models
{
    public class CommentViewModel
    {
        public string Author { get; set; }
        public int Rating { get; set; }
        public string Content { get; set; }
        public bool IsAuthor { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}