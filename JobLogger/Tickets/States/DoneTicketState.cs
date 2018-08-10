using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobLogger.Tickets.States
{
    class DoneTicketState : TicketState
    {
        public DoneTicketState() : base("Done", "DONE")
        {
        }

        public override IEnumerable<TicketPropertyValuePair> GetPropertyValuePairs(Ticket ticket)
        {
            return new List<TicketPropertyValuePair>();
        }

        public override IEnumerable<TicketStateValidationMessage> ValidateTicket(Ticket ticket)
        {
            List<TicketStateValidationMessage> list = new List<TicketStateValidationMessage>();

            return list;
        }
    }
}
