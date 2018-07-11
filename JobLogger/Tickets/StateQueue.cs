using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JobLogger.Tickets.States;

namespace JobLogger.Tickets
{
    public class StateQueue : List<TicketState>
    {
        public string Name { get; private set; }

        public StateQueue(string name)
        {
            this.Name = name;
        }

        public TicketState FindTicketState(string code)
        {
            foreach (TicketState state in this)
            {
                if (state.Code.Equals(code, StringComparison.OrdinalIgnoreCase))
                {
                    return state;
                }
            }

            return null;
        }
    }
}
