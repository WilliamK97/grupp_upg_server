using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace graph_tutorial.Models
{
    public class Booking
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Person { get; set; }
        public DateTime StartDate { get; set; }
    }
}