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

            list.Add(new TicketStateValidationMessage(
                "You are done!",
                "You can now hide this ticket from your dashboard",
                TicketStateValidationMessageSeverity.ActionNeeded,
                new TicketStateValidationMessageAction("Hide", innerTicket => innerTicket.MarkAsDone()),
                new TicketStateValidationMessageAction("Reopen - back to estimating", innerTicket => innerTicket.ReopenToEstimating())));

            if (!ticket.TracTicket.SprintAssignment.Equals("estimated", StringComparison.Ordinal) || !ticket.TracTicket.SprintAssignment.Equals("ready-for-sprint-verified-by-programmer", StringComparison.Ordinal))
            {
                list.Add(new TicketStateValidationMessage($"Should be in estimated or ready-for-sprint-verified-by-programmer (not {ticket.TracTicket.SprintAssignment})", "Ticket should be in the estimated or ready-for-sprint-verified-by-programmer", TicketStateValidationMessageSeverity.ActionNeeded));
            }

            if (ticket.TracTicket.Remaining != 0)
            {
                list.Add(new TicketStateValidationMessage("Remaining shouldn't be 0", "You should set an estimate", TicketStateValidationMessageSeverity.ActionNeeded));
            }

            return list;
        }
    }
}
