using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobLogger.Tickets.States
{
    class EstimatedTicketState : TicketState
    {
        public EstimatedTicketState() : base("Estimated", "ESTD")
        {
        }

        public override IEnumerable<TicketPropertyValuePair> GetPropertyValuePairs(Ticket ticket)
        {
            return new List<TicketPropertyValuePair>()
            {
                new TicketPropertyValuePair("Target version: ", ticket.TracTicket.TargetVersion),
                new TicketPropertyValuePair("Business value: ", ticket.TracTicket.BusinessValue.ToString()),
            };
        }

        public override IEnumerable<TicketStateValidationMessage> ValidateTicket(Ticket ticket)
        {
            List<TicketStateValidationMessage> list = new List<TicketStateValidationMessage>();

            if (!ticket.TracTicket.SprintAssignment.Equals("estimated", StringComparison.Ordinal) || !ticket.TracTicket.SprintAssignment.Equals("ready-for-sprint-verified-by-programmer", StringComparison.Ordinal))
            {
                list.Add(new TicketStateValidationMessage($"Should be in estimated or ready-for-sprint-verified-by-programmer (not {ticket.TracTicket.SprintAssignment})", "Ticket should be in the estimated or ready-for-sprint-verified-by-programmer", TicketStateValidationMessageSeverity.Warning));
            }

            if (ticket.TracTicket.Remaining < 1)
            {
                list.Add(new TicketStateValidationMessage("Remaining smaller than 1", "You should set an estimate", TicketStateValidationMessageSeverity.Warning));
            }

            return list;
        }

        public override bool IsDone(Ticket ticket)
        {
            return !this.ValidateTicket(ticket).Any();
        }
    }
}
