using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MetaTracInterface;

namespace JobLogger.Tickets.States
{
    class AcceptedTicketState : TicketState
    {
        public AcceptedTicketState() : base("Accepted", "ACC")
        {
        }

        public override IEnumerable<TicketPropertyValuePair> GetPropertyValuePairs(Ticket ticket)
        {
            return new List<TicketPropertyValuePair>()
            {
                new TicketPropertyValuePair("Status", ticket.TracTicket.Status.ToString()),
                new TicketPropertyValuePair("Target version: ", ticket.TracTicket.TargetVersion),
                new TicketPropertyValuePair("Business value: ", ticket.TracTicket.BusinessValue.ToString()),
                new TicketPropertyValuePair("Total hours: ", ticket.TracTicket.TotalHours.ToString()),
            };
        }

        public override IEnumerable<TicketStateValidationMessage> ValidateTicket(Ticket ticket)
        {
            List<TicketStateValidationMessage> list = new List<TicketStateValidationMessage>();

            list.AddRange(CommonValidations.Validate(
                ticket,
                CommonValidations.RemainingShouldBeGreaterThanZero,
                CommonValidations.ShouldBeInSprint));

            if (ticket.TracTicket.Status != TicketStatus.Accepted)
            {
                list.Add(new TicketStateValidationMessage("Should be accepted", "Incorrect status", TicketStateValidationMessageSeverity.Warning));
            }

            if (string.IsNullOrWhiteSpace(ticket.TracTicket.FeatureBranch))
            {
                list.Add(new TicketStateValidationMessage("Missing feature branch", "You should get a feature branch", TicketStateValidationMessageSeverity.Info));
            }

            return list;
        }

        public override bool IsDone(Ticket ticket)
        {
            return false;
        }
    }
}
