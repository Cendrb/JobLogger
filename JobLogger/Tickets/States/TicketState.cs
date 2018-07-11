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

        public virtual string GetStateString(Ticket ticket)
        {
            switch (ticket.TracTicket.Status)
            {
                case TicketStatus.New:
                    return "new";
                case TicketStatus.Assigned:
                    return "assigned";
                case TicketStatus.Accepted:
                    return "accepted";
                case TicketStatus.CodeReview:
                    return "code review";
                case TicketStatus.CodeReviewPassed:
                    return "CR passed";
                case TicketStatus.CodeReviewFailed:
                    return "CR failed";
                case TicketStatus.Testing:
                    return "testing";
                case TicketStatus.Reopened:
                    return "reopened";
                case TicketStatus.Closed:
                    return "closed";
                case TicketStatus.Documenting:
                    return "documenting";
                case TicketStatus.Unknown:
                    return "wtf";
                default:
                    return "wtf";
            }
        }

        public abstract IEnumerable<TicketPropertyValuePair> GetPropertyValuePairs(Ticket ticket);

        public abstract IEnumerable<TicketStateValidationMessage> ValidateTicket(Ticket ticket);

        public abstract bool IsDone(Ticket ticket);
    }
}
