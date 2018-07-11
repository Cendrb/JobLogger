using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobLogger.Tickets
{
    public class TicketStateValidationMessageAction
    {
        public string Title { get; private set; }
        private Action<Ticket> action;

        public TicketStateValidationMessageAction(string title, Action<Ticket> action)
        {
            this.Title = title;
            this.action = action;
        }

        public void Execute(Ticket ticket)
        {
            this.action(ticket);
        }
    }
}
