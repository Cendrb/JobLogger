using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobLogger.Tickets
{
    public interface IReadOnlyCustomTicketProperties
    {
        bool NoConfigurationRequired { get; }
        bool NoInstallationRequired { get; }
        bool POTQueuedUp { get; }
        bool POTDone { get; }
        bool EstimateMeetingOrganized { get; }
        bool SkipCodeReview { get; }
    }
}
