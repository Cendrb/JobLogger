using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobLogger.Tickets
{
    public class TicketStateValidationMessage
    {
        public string Title { get; private set; }
        public string Message { get; private set; }
        public TicketStateValidationMessageSeverity Severity { get; private set; }
        public IReadOnlyList<TicketStateValidationMessageAction> Actions { get; private set; }

        public TicketStateValidationMessage(string title, string message, TicketStateValidationMessageSeverity severity, params TicketStateValidationMessageAction[] actions)
        {
            this.Title = title;
            this.Message = message;
            this.Severity = severity;
            this.Actions = new List<TicketStateValidationMessageAction>(actions);
        }
    }
}
