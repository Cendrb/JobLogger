using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MetaTracInterface;

namespace JobLogger.Tickets.States
{
    class MergedTicketState : TicketState
    {
        public MergedTicketState() : base("Merged", "MERG")
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
                CommonValidations.ShouldBeInSprint,
                CommonValidations.FeatureBranchShouldBePresent,
                CommonValidations.TechnicalNotesShouldBePresent,
                CommonValidations.InstallationNotesShouldBePresent,
                CommonValidations.ConfigurationSettingsShouldBePresent,
                CommonValidations.HowToQAShouldBePresent,
                CommonValidations.TesterShouldBeAssigned));

            list.Add(new TicketStateValidationMessage(
                "Check the build",
                "Check whether the build has finished",
                TicketStateValidationMessageSeverity.ActionNeeded,
                new TicketStateValidationMessageAction("Successful", innerTicket => innerTicket.BuildFinishedSuccessfully()),
                new TicketStateValidationMessageAction("Failed", innerTicket => innerTicket.BuildFailed())));

            if (ticket.TracTicket.Status != TicketStatus.CodeReviewPassed && !ticket.TicketProperties.SkipCodeReview)
            {
                list.Add(new TicketStateValidationMessage($"Should be code_review_passed (not {ticket.TracTicket.Status.ToString()})", "Incorrect status", TicketStateValidationMessageSeverity.ActionNeeded));
            }

            return list;
        }
    }
}
