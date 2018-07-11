using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JobLogger.Tickets.States;
using MetaTracInterface;

namespace JobLogger.Tickets
{
    public class Ticket
    {
        public Ticket(IReadonlyTracTicket tracTicket, TicketState currentState, CustomTicketProperties ticketProperties, StateQueue stateQueue)
        {
            this.TracTicket = tracTicket;
            this.CurrentState = currentState;
            this.TicketProperties = ticketProperties;
            this.StateQueue = stateQueue;
        }

        public IReadonlyTracTicket TracTicket { get; set; }
        public TicketState CurrentState { get; set; }
        public CustomTicketProperties TicketProperties { get; set; }
        public StateQueue StateQueue { get; set; }

        public string GetPrimaryString()
        {
            return this.CurrentState.GetPrimaryString(this);
        }

        public string GetStateString()
        {
            return this.CurrentState.GetStateString(this);
        }

        public IEnumerable<TicketPropertyValuePair> GetPropertyValuePairs()
        {
            return this.CurrentState.GetPropertyValuePairs(this);
        }

        public bool IsDone()
        {
            return this.CurrentState.IsDone(this);
        }

        public IEnumerable<TicketStateValidationMessage> ValidateTicket()
        {
            return this.CurrentState.ValidateTicket(this);
        }
    }
}
