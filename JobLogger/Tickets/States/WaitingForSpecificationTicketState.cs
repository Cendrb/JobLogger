using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MetaTracInterface;

namespace JobLogger.Tickets.States
{
    class WaitingForSpecificationTicketState : TicketState
    {
        public WaitingForSpecificationTicketState() : base("Waiting for specification", "WFS")
        {
        }

        public override IEnumerable<TicketPropertyValuePair> GetPropertyValuePairs(Ticket ticket)
        {
            return new List<TicketPropertyValuePair>()
            {
                new TicketPropertyValuePair("Status", ticket.TracTicket.Status.ToString()),
                new TicketPropertyValuePair("Waiting for: ", $"{ticket.TicketProperties.WaitingForName} - {ticket.TicketProperties.WaitingForMessage}"),
                new TicketPropertyValuePair("Target version: ", ticket.TracTicket.TargetVersion),
                new TicketPropertyValuePair("Business value: ", ticket.TracTicket.BusinessValue.ToString()),
            };
        }

        public override IEnumerable<TicketStateValidationMessage> ValidateTicket(Ticket ticket)
        {
            List<TicketStateValidationMessage> list = new List<TicketStateValidationMessage>();
            if (!ticket.TracTicket.StatusUpdates.Any(statusUpdate => statusUpdate.Text.IndexOf("waiting", StringComparison.OrdinalIgnoreCase) > -1))
            {
                list.Add(new TicketStateValidationMessage("Add a status about who are you waiting for", "You'd better add a status update that you are waiting for someone", TicketStateValidationMessageSeverity.Info));
            }

            if (!ticket.TracTicket.SprintAssignment.Equals("need-more-info-from-product-owner", StringComparison.Ordinal))
            {
                list.Add(new TicketStateValidationMessage($"Should be in need-more-info-from-product-owner (not {ticket.TracTicket.SprintAssignment})", "Ticket should be in the need-more-info-from-product-owner sprint", TicketStateValidationMessageSeverity.Info));
            }

            return list;
        }

        public override bool IsDone(Ticket ticket)
        {
            return false;
        }
    }
}
