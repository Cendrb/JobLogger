using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MetaTracInterface;

namespace JobLogger.Tickets.States
{
    class ProgrammingTicketState : TicketState
    {
        public ProgrammingTicketState() : base("Accepted", "PROG")
        {
        }

        public override IEnumerable<TicketPropertyValuePair> GetPropertyValuePairs(Ticket ticket)
        {
            return new List<TicketPropertyValuePair>()
            {
                new TicketPropertyValuePair("Status", ticket.TracTicket.Status.ToString()),
                new TicketPropertyValuePair("Target version", ticket.TracTicket.TargetVersion),
                new TicketPropertyValuePair("Business value", ticket.TracTicket.BusinessValue.ToString()),
                new TicketPropertyValuePair("Total hours", ticket.TracTicket.TotalHours.ToString()),
            };
        }

        public override IEnumerable<TicketStateValidationMessage> ValidateTicket(Ticket ticket)
        {
            List<TicketStateValidationMessage> list = new List<TicketStateValidationMessage>();

            list.AddRange(CommonValidations.Validate(
                ticket,
                CommonValidations.RemainingShouldBeGreaterThanZero,
                CommonValidations.ShouldBeInSprint));

            list.Add(new TicketStateValidationMessage(
                "Programming",
                "Come on, do it.",
                TicketStateValidationMessageSeverity.ActionNeeded,
                new TicketStateValidationMessageAction("Done", innerTicket => innerTicket.ProgrammingDone()),
                new TicketStateValidationMessageAction("Incomplete specification", innerTicket => innerTicket.IncompleteSpecificationForProgramming())));

            if (ticket.TracTicket.Status != TicketStatus.Accepted)
            {
                list.Add(new TicketStateValidationMessage($"Should be accepted (not {ticket.TracTicket.Status.ToString()})", "Incorrect status", TicketStateValidationMessageSeverity.ActionNeeded));
            }

            return list;
        }
    }
}
