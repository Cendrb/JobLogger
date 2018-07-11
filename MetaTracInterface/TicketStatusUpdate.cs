using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetaTracInterface
{
    public class TicketStatusUpdate
    {
        public DateTime? DateTime { get; set; }
        public string AuthorAbbreviation { get; set; }
        public string Text { get; set; }
    }
}
