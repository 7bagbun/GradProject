using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApp.Models.ViewModels
{
    public class ItemListViewModel
    {
        public IEnumerable<Selling> Items { get; set; }
        public Comment Comment { get; set; }
    }
}