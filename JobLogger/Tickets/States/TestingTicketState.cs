using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MetaTracInterface;

namespace JobLogger.Tickets.States
{
    class TestingTicketState : TicketState
    {
        public TestingTicketState() : base("Testing", "TEST")
        {
        }

        public override IEnumerable<TicketPropertyValuePair> GetPropertyValuePairs(Ticket ticket)
        {
            return new List<TicketPropertyValuePair>()
            {
                new TicketPropertyValuePair("Status", ticket.TracTicket.Status.ToString()),
                new TicketPropertyValuePair("QA by: ", ticket.TracTicket.QaBY),
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
                CommonValidations.FeatureBranchShouldBePresent,
                CommonValidations.TechnicalNotesShouldBePresent,
                CommonValidations.InstallationNotesShouldBePresent,
                CommonValidations.ConfigurationSettingsShouldBePresent,
                CommonValidations.HowToQAShouldBePresent,
                CommonValidations.TesterShouldBeAssigned,
                CommonValidations.MilestoneShouldBeAssigned));

            if (ticket.TracTicket.Status != TicketStatus.CodeReviewPassed)
            {
                list.Add(new TicketStateValidationMessage("Should be code_review_passed", "Incorrect status", TicketStateValidationMessageSeverity.Warning));
            }

            if (ticket.TracTicket.Status != TicketStatus.Documenting || ticket.TracTicket.Status != TicketStatus.Closed)
            {
                if (ticket.TracTicket.Status == TicketStatus.Reopened)
                {
                    list.Add(new TicketStateValidationMessage("Status", "Ticket reopened. Get back to work!", TicketStateValidationMessageSeverity.Alert));
                }
                else
                {
                    if (!ticket.TicketProperties.POTDone)
                    {
                        if (ticket.TracTicket.Status != TicketStatus.Testing || ticket.TracTicket.Status != TicketStatus.CodeReviewPassed)
                        {
                            list.Add(new TicketStateValidationMessage("Status", "Ticket should be testing or in code_review_passed (when doing preliminary POT)", TicketStateValidationMessageSeverity.Warning));
                        }
                    }
                    else
                    {
                        if (ticket.TracTicket.Status == TicketStatus.CodeReviewPassed)
                        {
                            list.Add(new TicketStateValidationMessage("Status", "POT is done. Switch this into testing", TicketStateValidationMessageSeverity.Alert));
                        }
                        else if (ticket.TracTicket.Status != TicketStatus.Testing)
                        {
                            list.Add(new TicketStateValidationMessage("Status", "Ticket should be in testing", TicketStateValidationMessageSeverity.Warning));
                        }
                    }
                }
            }

            if (!ticket.TicketProperties.POTDone && !ticket.TicketProperties.POTQueuedUp)
            {
                list.Add(new TicketStateValidationMessage("POT", "You should ask for POT", TicketStateValidationMessageSeverity.Warning));
            }

            if (!ticket.TracTicket.TestPlans.Any())
            {
                list.Add(new TicketStateValidationMessage("Test plans", "No test plans attach. Send a mesage to a tester to create one.", TicketStateValidationMessageSeverity.Warning));
            }
            else
            {
                if (!ticket.TracTicket.TestPlanReviewed)
                {
                    list.Add(new TicketStateValidationMessage("Test plans", "There is a test plan attached but you haven't verified it yet.", TicketStateValidationMessageSeverity.Alert));
                }
            }

            return list;
        }

        public override bool IsDone(Ticket ticket)
        {
            return !this.ValidateTicket(ticket).Any();
        }
    }
}
