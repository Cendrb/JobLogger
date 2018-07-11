using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobLogger.Tickets.States
{
    class CommonValidations
    {
        public static IEnumerable<TicketStateValidationMessage> Validate(Ticket ticket, params Func<Ticket, IEnumerable<TicketStateValidationMessage>>[] validationFunctions)
        {
            List<TicketStateValidationMessage> list = new List<TicketStateValidationMessage>();
            foreach (Func<Ticket, IEnumerable<TicketStateValidationMessage>> validationFunction in validationFunctions)
            {
                list.AddRange(validationFunction(ticket));
            }

            return list;
        }

        public static IEnumerable<TicketStateValidationMessage> RemainingShouldBeGreaterThanZero(Ticket ticket)
        {
            List<TicketStateValidationMessage> list = new List<TicketStateValidationMessage>();

            if (ticket.TracTicket.Remaining < 1)
            {
                list.Add(new TicketStateValidationMessage("Remaining smaller than 1", "Ticket should have more than 0 in the Remaining field", TicketStateValidationMessageSeverity.Warning));
            }

            return list;
        }

        public static IEnumerable<TicketStateValidationMessage> ShouldBeInSprint(Ticket ticket)
        {
            List<TicketStateValidationMessage> list = new List<TicketStateValidationMessage>();

            if (!ticket.TracTicket.SprintAssignment.StartsWith("sprint", StringComparison.OrdinalIgnoreCase))
            {
                list.Add(new TicketStateValidationMessage("Not in a sprint", "Ticket should be in a sprint", TicketStateValidationMessageSeverity.Warning));
            }

            return list;
        }

        public static IEnumerable<TicketStateValidationMessage> FeatureBranchShouldBePresent(Ticket ticket)
        {
            List<TicketStateValidationMessage> list = new List<TicketStateValidationMessage>();

            if (string.IsNullOrWhiteSpace(ticket.TracTicket.FeatureBranch))
            {
                list.Add(new TicketStateValidationMessage("Missing feature branch", "You should get a feature branch", TicketStateValidationMessageSeverity.Warning));
            }

            return list;

        }

        public static IEnumerable<TicketStateValidationMessage> TechnicalNotesShouldBePresent(Ticket ticket)
        {
            List<TicketStateValidationMessage> list = new List<TicketStateValidationMessage>();

            if (string.IsNullOrWhiteSpace(ticket.TracTicket.TechnicalNotes))
            {
                list.Add(new TicketStateValidationMessage("Missing technical notes", "Technical notes should be present", TicketStateValidationMessageSeverity.Warning));
            }

            return list;
        }

        public static IEnumerable<TicketStateValidationMessage> ConfigurationSettingsShouldBePresent(Ticket ticket)
        {
            List<TicketStateValidationMessage> list = new List<TicketStateValidationMessage>();

            if (!ticket.TicketProperties.NoConfigurationRequired && string.IsNullOrWhiteSpace(ticket.TracTicket.ConfigurationSettings))
            {
                list.Add(new TicketStateValidationMessage("Missing configuration settings", "Either add configuration settings or select No configuration required", TicketStateValidationMessageSeverity.Warning));
            }

            return list;
        }

        public static IEnumerable<TicketStateValidationMessage> InstallationNotesShouldBePresent(Ticket ticket)
        {
            List<TicketStateValidationMessage> list = new List<TicketStateValidationMessage>();

            if (!ticket.TicketProperties.NoInstallationRequired && string.IsNullOrWhiteSpace(ticket.TracTicket.InstallationNotes))
            {
                list.Add(new TicketStateValidationMessage("Missing installation notes", "Either add installation notes or select No installation required", TicketStateValidationMessageSeverity.Warning));
            }

            return list;
        }

        public static IEnumerable<TicketStateValidationMessage> HowToQAShouldBePresent(Ticket ticket)
        {
            List<TicketStateValidationMessage> list = new List<TicketStateValidationMessage>();

            if (string.IsNullOrWhiteSpace(ticket.TracTicket.HowToQA))
            {
                list.Add(new TicketStateValidationMessage("How to QA missing", "How to QA should be present", TicketStateValidationMessageSeverity.Warning));
            }

            return list;
        }

        public static IEnumerable<TicketStateValidationMessage> TesterShouldBeAssigned(Ticket ticket)
        {
            List<TicketStateValidationMessage> list = new List<TicketStateValidationMessage>();

            if (string.IsNullOrWhiteSpace(ticket.TracTicket.QaBY) || ticket.TracTicket.QaBY.Equals("--Please select--", StringComparison.OrdinalIgnoreCase))
            {
                list.Add(new TicketStateValidationMessage("No tester assigned", "Get a tester assigned to this", TicketStateValidationMessageSeverity.Warning));
            }

            return list;
        }

        public static IEnumerable<TicketStateValidationMessage> MilestoneShouldBeAssigned(Ticket ticket)
        {
            List<TicketStateValidationMessage> list = new List<TicketStateValidationMessage>();

            if (!ticket.TracTicket.Milestone.Contains("."))
            {
                list.Add(new TicketStateValidationMessage("Milestone", "You need to assign a milestone", TicketStateValidationMessageSeverity.Warning));
            }

            return list;
        }
    }
}
