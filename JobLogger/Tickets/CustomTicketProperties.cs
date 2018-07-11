using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobLogger.Tickets
{
    public class CustomTicketProperties
    {
        public string WaitingForName { get; set; }
        public string WaitingForMessage { get; set; }
        public bool NoConfigurationRequired { get; set; }
        public bool NoInstallationRequired { get; set; }
        public bool POTQueuedUp { get; set; }
        public bool POTDone { get; set; }
    }
}
