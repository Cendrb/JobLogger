using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobLogger.Tickets
{
    class TicketSerializableData
    {
        public int ID { get; set; }
        public string StatusCode { get; set; }
        public string StateQueue { get; set; }
        public CustomTicketProperties TicketProperties { get; set; }
    }
}
