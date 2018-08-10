using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MetaTracInterface;

namespace JobLogger.Tickets.States
{
    class WaitingForProgrammingSpecificationTicketState : TicketState
    {
        public WaitingForProgrammingSpecificationTicketState() : base("Waiting for specification", "WFSP")
        {
        }

        public override IEnumerable<TicketPropertyValuePair> GetPropertyValuePairs(Ticket ticket)
        {
            return new List<TicketPropertyValuePair>()
            {
                new TicketPropertyValuePair("Status", ticket.TracTicket.Status.ToString()),
                new TicketPropertyValuePair("Target version: ", ticket.TracTicket.TargetVersion),
                new TicketPropertyValuePair("Business value: ", ticket.TracTicket.BusinessValue.ToString()),
            };
        }

        public override IEnumerable<TicketStateValidationMessage> ValidateTicket(Ticket ticket)
        {
            List<TicketStateValidationMessage> list = new List<TicketStateValidationMessage>();

            list.Add(new TicketStateValidationMessage(
                "Waiting for specification",
                "Waiting for the product owner to provide more info",
                TicketStateValidationMessageSeverity.Waiting,
                new TicketStateValidationMessageAction("Specification received", innerTicket => innerTicket.ProgrammingSpecificationReceived())));

            if (!ticket.TracTicket.StatusUpdates.Any(statusUpdate => statusUpdate.Text.IndexOf("waiting", StringComparison.OrdinalIgnoreCase) > -1))
            {
                list.Add(new TicketStateValidationMessage("Add a status about who are you waiting for", "You'd better add a status update that you are waiting for someone", TicketStateValidationMessageSeverity.ActionNeeded));
            }

            if (!ticket.TracTicket.SprintAssignment.Equals("need-more-info-from-product-owner", StringComparison.Ordinal))
            {
                list.Add(new TicketStateValidationMessage($"Should be in need-more-info-from-product-owner (not {ticket.TracTicket.SprintAssignment})", "Ticket should be in the need-more-info-from-product-owner sprint", TicketStateValidationMessageSeverity.ActionNeeded));
            }

            return list;
        }
    }
}
