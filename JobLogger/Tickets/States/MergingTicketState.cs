using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MetaTracInterface;

namespace JobLogger.Tickets.States
{
    class MergingTicketState : TicketState
    {
        public MergingTicketState() : base("Merging", "MERG")
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

            if (ticket.TracTicket.Status != TicketStatus.CodeReviewPassed)
            {
                list.Add(new TicketStateValidationMessage("Should be code_review_passed", "Incorrect status", TicketStateValidationMessageSeverity.Warning));
            }

            return list;
        }

        public override bool IsDone(Ticket ticket)
        {
            return false;
        }
    }
}
