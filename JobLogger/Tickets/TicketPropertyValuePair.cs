using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobLogger.Tickets
{
    public class TicketPropertyValuePair
    {
        public string Name { get; private set; }
        public string Value { get; private set; }

        public TicketPropertyValuePair(string name, string value)
        {
            this.Name = name;
            this.Value = value;
        }
    }
}
