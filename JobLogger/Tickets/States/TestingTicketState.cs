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

            if (ticket.TracTicket.Status != TicketStatus.Documenting && ticket.TracTicket.Status != TicketStatus.Closed)
            {
                if (ticket.TracTicket.Status == TicketStatus.Reopened)
                {
                    list.Add(new TicketStateValidationMessage(
                        "Ticket reopened",
                        "Ticket reopened. Get back to work!",
                        TicketStateValidationMessageSeverity.ImmediateActionRequired));
                }
                else if (ticket.TracTicket.Status != TicketStatus.Testing)
                {
                    list.Add(new TicketStateValidationMessage($"Should be in testing (not {ticket.TracTicket.Status.ToString()})", "Ticket should be testing", TicketStateValidationMessageSeverity.ActionNeeded));
                }
            }

            if (!ticket.TicketProperties.POTDone)
            {
                if (ticket.TicketProperties.POTQueuedUp)
                {
                    list.Add(new TicketStateValidationMessage(
                        "POT in progress",
                        "You've sent a request for POT of this ticket. Mark this as done when response comes back.",
                        TicketStateValidationMessageSeverity.Waiting,
                        new TicketStateValidationMessageAction("Done", innerTicket => innerTicket.POTSuccessful()),
                        new TicketStateValidationMessageAction("Failed", innerTicket => innerTicket.POTFailed())));
                }
                else
                {
                    list.Add(new TicketStateValidationMessage(
                        "Ask for POT",
                        "You should ask for POT",
                        TicketStateValidationMessageSeverity.ActionNeeded,
                        new TicketStateValidationMessageAction("Done", innerTicket => innerTicket.AskedForPOT())));
                }
            }

            if (!ticket.TracTicket.TestPlans.Any())
            {
                list.Add(new TicketStateValidationMessage("Waiting for test plan", "No test plans attached. Send a mesage to a tester to create one.", TicketStateValidationMessageSeverity.Waiting));
            }
            else
            {
                if (!ticket.TracTicket.TestPlanReviewed)
                {
                    list.Add(new TicketStateValidationMessage($"Test plan (${ticket.TracTicket.TestPlans.First().FileName}) not reviewed", "There is a test plan attached but you haven't verified it yet.", TicketStateValidationMessageSeverity.ImmediateActionRequired));
                }
                else if (ticket.TracTicket.Status == TicketStatus.Documenting)
                {
                    list.Add(new TicketStateValidationMessage(
                        "Done from your part! (documenting)",
                        "Waiting for the tester to create documentation",
                        TicketStateValidationMessageSeverity.Waiting,
                        new TicketStateValidationMessageAction("Hide", innerTicket => innerTicket.MarkAsDone()),
                        new TicketStateValidationMessageAction("Reopen", innerTicket => innerTicket.ReopenToProgramming())));
                }
                else if (ticket.TracTicket.Status == TicketStatus.Testing)
                {
                    list.Add(new TicketStateValidationMessage(
                        "Waiting for testing",
                        "Waiting for the tester to go through the test cases",
                        TicketStateValidationMessageSeverity.Waiting,
                        new TicketStateValidationMessageAction("Reopen", innerTicket => innerTicket.ReopenToProgramming())));
                }
                else if (ticket.TracTicket.Status == TicketStatus.Closed && ticket.TicketProperties.POTDone)
                {
                    list.Add(new TicketStateValidationMessage(
                        "You are done!",
                        "You can now hide this ticket from your dashboard",
                        TicketStateValidationMessageSeverity.ActionNeeded,
                        new TicketStateValidationMessageAction("Hide", innerTicket => innerTicket.MarkAsDone()),
                        new TicketStateValidationMessageAction("Reopen", innerTicket => innerTicket.ReopenToProgramming())));
                }
            }

            return list;
        }
    }
}
