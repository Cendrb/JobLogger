using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MetaTracInterface;

namespace JobLogger.Tickets.States
{
    class InternalCodeReviewTicketState : TicketState
    {
        public InternalCodeReviewTicketState() : base("Internal code review", "ICR")
        {
        }

        public override IEnumerable<TicketPropertyValuePair> GetPropertyValuePairs(Ticket ticket)
        {
            return new List<TicketPropertyValuePair>()
            {
                new TicketPropertyValuePair("Status", ticket.TracTicket.Status.ToString()),
                new TicketPropertyValuePair("Feature branch", ticket.TracTicket.FeatureBranch),
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
               CommonValidations.FeatureBranchShouldBePresent,
               CommonValidations.TechnicalNotesShouldBePresent,
               CommonValidations.ConfigurationSettingsShouldBePresent,
               CommonValidations.InstallationNotesShouldBePresent,
               CommonValidations.HowToQAShouldBePresent,
               CommonValidations.TesterShouldBeAssigned,
               CommonValidations.MilestoneShouldBeAssigned));

            if (ticket.TracTicket.Status != TicketStatus.CodeReview)
            {
                list.Add(new TicketStateValidationMessage("Should be code_review", "Incorrect status", TicketStateValidationMessageSeverity.Warning));
            }

            if (ticket.TracTicket.Status == TicketStatus.CodeReviewPassed)
            {
                list.Add(new TicketStateValidationMessage("CR passed, merge the ticket", "Your ticket passed code review. Merge it.", TicketStateValidationMessageSeverity.Alert));
            }

            if (ticket.TracTicket.Status == TicketStatus.CodeReviewFailed)
            {
                list.Add(new TicketStateValidationMessage("CR failed", "Your ticket failed code review. Time to go back to development.", TicketStateValidationMessageSeverity.Alert));
            }

            return list;
        }

        public override bool IsDone(Ticket ticket)
        {
            return false;
        }
    }
}
