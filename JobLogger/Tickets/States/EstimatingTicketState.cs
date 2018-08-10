using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobLogger.Tickets.States
{
    class EstimatingTicketState : TicketState
    {
        public EstimatingTicketState() : base("Estimating", "EST")
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
                "Estimating",
                "No one is gonna do this for you",
                TicketStateValidationMessageSeverity.ActionNeeded,
                new TicketStateValidationMessageAction("Done", innerTicket => innerTicket.Estimated()),
                new TicketStateValidationMessageAction("Incomplete specification", innerTicket => innerTicket.IncompleteSpecificationForEstimating())));

            if (!ticket.TracTicket.SprintAssignment.Equals("estimate-needed", StringComparison.Ordinal) || !ticket.TracTicket.SprintAssignment.Equals("ready-for-sprint", StringComparison.Ordinal))
            {
                list.Add(new TicketStateValidationMessage($"Should be in estimate-needed or ready-for-sprint (not {ticket.TracTicket.SprintAssignment})", "Ticket should be in the estimate-needed or ready-for-sprint", TicketStateValidationMessageSeverity.ActionNeeded));
            }

            return list;
        }
    }
}
