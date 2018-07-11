using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MetaTracInterface;

namespace JobLogger.Tickets.States
{
    class NewTicketState : TicketState
    {
        public NewTicketState() : base("New", "NEW")
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

            list.AddRange(CommonValidations.Validate(
                ticket,
                CommonValidations.ShouldBeInSprint,
                CommonValidations.RemainingShouldBeGreaterThanZero));


            if (ticket.TracTicket.Remaining < 1)
            {
                list.Add(new TicketStateValidationMessage("Remaining", "Ticket should have an estimate", TicketStateValidationMessageSeverity.Warning));
            }

            if (ticket.TracTicket.Status != TicketStatus.New && ticket.TracTicket.Status != TicketStatus.Assigned)
            {
                list.Add(new TicketStateValidationMessage("Incorrect status", "Ticket should be new or assigned", TicketStateValidationMessageSeverity.Warning));
            }

            return list;
        }

        public override bool IsDone(Ticket ticket)
        {
            return false;
        }
    }
}
