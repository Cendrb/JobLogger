using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobLogger.Tickets
{
    public class TicketStateValidationMessage
    {
        public string AffectedField { get; private set; }
        public string Message { get; private set; }
        public TicketStateValidationMessageSeverity Severity { get; private set; }
        public IReadOnlyList<TicketStateValidationMessageAction> Actions { get; private set; }

        public TicketStateValidationMessage(string affectedField, string message, TicketStateValidationMessageSeverity severity, params TicketStateValidationMessageAction[] actions)
        {
            this.AffectedField = affectedField;
            this.Message = message;
            this.Severity = severity;
            this.Actions = new List<TicketStateValidationMessageAction>(actions);
        }
    }
}
