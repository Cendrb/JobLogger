using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JobLogger.Tickets.States;

namespace JobLogger.Tickets
{
    class StateQueues : List<StateQueue>
    {
        public TicketState FindTicketState(string code)
        {
            foreach(StateQueue queue in this)
            {
                TicketState ticketState = queue.FindTicketState(code);
                if(ticketState != null)
                {
                    return ticketState;
                }
            }

            return null;
        }
    }
}
