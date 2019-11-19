using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace graph_tutorial.Models
{
    public class ListDetailsViewModel
    {
        public string Title { get; set; }
        public IEnumerable<string> Items { get; set; }
    }
}