using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobLogger.Tickets.States
{
    class EstimatingMeetingTicketState : TicketState
    {
        public EstimatingMeetingTicketState() : base("Estimating meeting", "ESTM")
        {
        }

        public override IEnumerable<TicketPropertyValuePair> GetPropertyValuePairs(Ticket ticket)
        {
            return new List<TicketPropertyValuePair>()
            {
                new TicketPropertyValuePair("Target version: ", ticket.TracTicket.TargetVersion),
                new TicketPropertyValuePair("Business value: ", ticket.TracTicket.BusinessValue.ToString()),
                new TicketPropertyValuePair("Estimate", ticket.TracTicket.Remaining.ToString())
            };
        }

        public override IEnumerable<TicketStateValidationMessage> ValidateTicket(Ticket ticket)
        {
            List<TicketStateValidationMessage> list = new List<TicketStateValidationMessage>();

            if (ticket.TicketProperties.EstimateMeetingOrganized)
            {
                list.Add(new TicketStateValidationMessage(
                    "Waiting for the estimate meeting",
                    "Just wait for it to happen",
                    TicketStateValidationMessageSeverity.Waiting,
                    new TicketStateValidationMessageAction("Passed", innerTicket => innerTicket.EstimateMeetingPassed()),
                    new TicketStateValidationMessageAction("Failed", innerTicket => innerTicket.EstimateMeetingFailed())));
            }
            else
            {
                list.Add(new TicketStateValidationMessage(
                    "Organize an estimate meeting",
                    "Send a message to Slack and organize a estimate meeting",
                    TicketStateValidationMessageSeverity.ActionNeeded,
                    new TicketStateValidationMessageAction("Done", innerTicket => innerTicket.EstimateMeetingOrganized()),
                    new TicketStateValidationMessageAction("Re-estimate", innerTicket => innerTicket.ReopenToEstimating())));
            }

            if (ticket.TracTicket.Remaining != 0)
            {
                list.Add(new TicketStateValidationMessage("Remaining shouldn't be 0", "You should set an estimate", TicketStateValidationMessageSeverity.ActionNeeded));
            }

            if (!ticket.TracTicket.SprintAssignment.Equals("estimate-needed", StringComparison.Ordinal) && !ticket.TracTicket.SprintAssignment.Equals("ready-for-sprint", StringComparison.Ordinal))
            {
                list.Add(new TicketStateValidationMessage($"Should be in estimate-needed or ready-for-sprint (not {ticket.TracTicket.SprintAssignment})", "Ticket should be in the estimate-needed or ready-for-sprint", TicketStateValidationMessageSeverity.ActionNeeded));
            }

            return list;
        }
    }
}
