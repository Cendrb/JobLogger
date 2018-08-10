using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobLogger.Tickets
{
    public class CustomTicketProperties : IReadOnlyCustomTicketProperties
    {
        public bool NoConfigurationRequired { get; set; }
        public bool NoInstallationRequired { get; set; }
        public bool POTQueuedUp { get; set; }
        public bool POTDone { get; set; }
        public bool EstimateMeetingOrganized { get; set; }
        public bool SkipCodeReview { get; set; }

        public static CustomTicketProperties CloneFrom(IReadOnlyCustomTicketProperties properties)
        {
            return new CustomTicketProperties()
            {
                EstimateMeetingOrganized = properties.EstimateMeetingOrganized,
                NoConfigurationRequired = properties.NoConfigurationRequired,
                NoInstallationRequired = properties.NoInstallationRequired,
                POTDone = properties.POTDone,
                POTQueuedUp = properties.POTQueuedUp,
                SkipCodeReview = properties.SkipCodeReview
            };
        }
    }
}
