using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApp.Models
{
    public class PriceHistoryModel
    {
        public DateTime UpdatedTime { get; set; }
        public int Price { get; set; }
    }
}