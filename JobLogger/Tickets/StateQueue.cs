using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JobLogger.Tickets.States;

namespace JobLogger.Tickets
{
    public class StateQueue
    {
        public string Name { get; }
        public TicketState InitialState { get; }

        public StateQueue(string name, TicketState initialState)
        {
            this.Name = name;
            this.InitialState = initialState;
        }
    }
}
