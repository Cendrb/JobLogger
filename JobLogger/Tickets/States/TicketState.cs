using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MetaTracInterface;

namespace JobLogger.Tickets.States
{
    public abstract class TicketState
    {
        public string Name { get; private set; }
        public string Code { get; private set; }

        public TicketState(string name, string code)
        {
            this.Name = name;
            this.Code = code;
        }

        public virtual string GetPrimaryString(Ticket ticket)
        {
            return $"#{ticket.TracTicket.ID} {ticket.TracTicket.Summary}";
        }

        public virtual string GetStatusUpdatesString(Ticket ticket)
        {
            return string.Join(Environment.NewLine, ticket.TracTicket.StatusUpdates.Select(statusUpdate => $"{statusUpdate.DateTime:MM/dd/yy} ({statusUpdate.AuthorAbbreviation}) {statusUpdate.Text}"));
        }

        public abstract IEnumerable<TicketPropertyValuePair> GetPropertyValuePairs(Ticket ticket);

        public abstract IEnumerable<TicketStateValidationMessage> ValidateTicket(Ticket ticket);

        public abstract bool IsDone(Ticket ticket);
    }
}
