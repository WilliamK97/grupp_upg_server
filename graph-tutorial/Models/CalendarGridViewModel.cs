using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace graph_tutorial.Models
{
    public class CalendarGridViewModel
    {
        public int Rows { get; set; }
        public int Cols { get; set; }
        public int Days { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
        public string MonthName { get; set; }
    }
}