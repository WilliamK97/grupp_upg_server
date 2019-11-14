using Microsoft.Graph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace graph_tutorial.Models
{
    public class CalendarDetailsViewModel
    {
        public IEnumerable<EventViewModel> Events { get; set; }
    }

    public class EventViewModel
    {
        public string Subject { get; set; }
        public string Organizer { get; set; }
        public string Start { get; set; }
        public string End { get; set; }
        public IEnumerable<Attendee> Attendees { get; set; }
    }
}