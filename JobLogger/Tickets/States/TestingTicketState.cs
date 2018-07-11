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

            if(ticket.TicketProperties.POTQueuedUp && !ticket.TicketProperties.POTDone)
            {
                list.Add(new TicketStateValidationMessage(
                    "POT in progress",
                    "You've sent a request for POT of this ticket. Mark this as done when response comes back.",
                    TicketStateValidationMessageSeverity.Info,
                    new TicketStateValidationMessageAction("Done", innerTicket => innerTicket.TicketProperties.POTDone = true),
                    new TicketStateValidationMessageAction("Failed", innerTicket => innerTicket.TicketProperties.POTQueuedUp = false)));
            }

            if (ticket.TracTicket.Status != TicketStatus.Documenting || ticket.TracTicket.Status != TicketStatus.Closed)
            {
                if (ticket.TracTicket.Status == TicketStatus.Reopened)
                {
                    list.Add(new TicketStateValidationMessage("Ticket reopened", "Ticket reopened. Get back to work!", TicketStateValidationMessageSeverity.Alert));
                }
                else
                {
                    if (!ticket.TicketProperties.POTDone)
                    {
                        if (ticket.TracTicket.Status != TicketStatus.Testing || ticket.TracTicket.Status != TicketStatus.CodeReviewPassed)
                        {
                            list.Add(new TicketStateValidationMessage($"Should be in testing or CR passed (not {ticket.TracTicket.Status.ToString()})", "Ticket should be testing or in code_review_passed (when doing preliminary POT)", TicketStateValidationMessageSeverity.Warning));
                        }
                    }
                    else
                    {
                        if (ticket.TracTicket.Status == TicketStatus.CodeReviewPassed)
                        {
                            list.Add(new TicketStateValidationMessage($"Should be in testing (not {ticket.TracTicket.Status.ToString()})", "POT is done. Switch this into testing", TicketStateValidationMessageSeverity.Alert));
                        }
                        else if (ticket.TracTicket.Status != TicketStatus.Testing)
                        {
                            list.Add(new TicketStateValidationMessage($"Should be in testing (not {ticket.TracTicket.Status.ToString()})", "Ticket should be in testing", TicketStateValidationMessageSeverity.Warning));
                        }
                    }
                }
            }

            if (!ticket.TicketProperties.POTDone && !ticket.TicketProperties.POTQueuedUp)
            {
                list.Add(new TicketStateValidationMessage("Ask for POT", "You should ask for POT", TicketStateValidationMessageSeverity.Warning, new TicketStateValidationMessageAction("Done", innerTicket => innerTicket.TicketProperties.POTQueuedUp = true)));
            }

            if (!ticket.TracTicket.TestPlans.Any())
            {
                list.Add(new TicketStateValidationMessage("No test plan", "No test plans attached. Send a mesage to a tester to create one.", TicketStateValidationMessageSeverity.Alert));
            }
            else
            {
                if (!ticket.TracTicket.TestPlanReviewed)
                {
                    list.Add(new TicketStateValidationMessage($"Test plan (${ticket.TracTicket.TestPlans.First().FileName}) not reviewed", "There is a test plan attached but you haven't verified it yet.", TicketStateValidationMessageSeverity.Alert));
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
