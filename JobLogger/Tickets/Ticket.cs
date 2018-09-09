using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JobLogger.Tickets.States;
using MetaTracInterface;

namespace JobLogger.Tickets
{
    public class Ticket
    {
        private CustomTicketProperties ticketProperties;

        public Ticket(TracTicket tracTicket, TicketState currentState, CustomTicketProperties ticketProperties)
        {
            this.TracTicket = tracTicket;
            this.CurrentState = currentState;
            this.ticketProperties = ticketProperties;
        }

        public TracTicket TracTicket { get; }
        public TicketState CurrentState { get; private set; }
        public IReadOnlyCustomTicketProperties TicketProperties { get { return this.ticketProperties; } }

        public string GetPrimaryString()
        {
            return this.CurrentState.GetPrimaryString(this);
        }

        public string GetStatusUpdatesString()
        {
            return this.CurrentState.GetStatusUpdatesString(this);
        }

        public IEnumerable<TicketPropertyValuePair> GetPropertyValuePairs()
        {
            return this.CurrentState.GetPropertyValuePairs(this);
        }

        public IEnumerable<TicketStateValidationMessage> ValidateTicket()
        {
            return this.CurrentState.ValidateTicket(this);
        }

        public void SkipCodeReview()
        {
            this.ticketProperties.SkipCodeReview = true;
        }

        public void ReopenToProgramming()
        {
            this.CurrentState = TicketStateRegistry.Instance.Get<ProgrammingTicketState>();
            this.ticketProperties.POTDone = false;
            this.ticketProperties.POTQueuedUp = false;
            this.ticketProperties.NoConfigurationRequired = false;
            this.ticketProperties.NoInstallationRequired = false;
        }

        public void ReopenToEstimating()
        {
            this.CurrentState = TicketStateRegistry.Instance.Get<EstimatingTicketState>();
            this.ticketProperties.EstimateMeetingOrganized = false;
        }

        public void Merge()
        {
            this.CurrentState = TicketStateRegistry.Instance.Get<MergedTicketState>();
        }

        public void MarkAsDone()
        {
            this.CurrentState = TicketStateRegistry.Instance.Get<DoneTicketState>();
        }

        public void InternalCodeReviewPassed()
        {
            this.CurrentState = TicketStateRegistry.Instance.Get<CodeReviewTicketState>();
        }

        public void ProgrammingDone()
        {
            this.CurrentState = TicketStateRegistry.Instance.Get<InternalCodeReviewTicketState>();
            this.ticketProperties.POTDone = false;
            this.ticketProperties.POTQueuedUp = false;
        }

        public void IncompleteSpecificationForProgramming()
        {
            this.CurrentState = TicketStateRegistry.Instance.Get<WaitingForProgrammingSpecificationTicketState>();
        }

        public void IncompleteSpecificationForEstimating()
        {
            this.CurrentState = TicketStateRegistry.Instance.Get<WaitingForEstimatingSpecificationTicketState>();
        }

        public void POTSuccessful()
        {
            this.ticketProperties.POTDone = true;
            this.ticketProperties.POTQueuedUp = false;
        }

        public void POTFailed()
        {
            this.ReopenToProgramming();
            this.ticketProperties.POTDone = false;
            this.ticketProperties.POTQueuedUp = false;
        }

        public void AskedForPOT()
        {
            this.ticketProperties.POTQueuedUp = true;
        }

        public void NoConfigurationNeeded()
        {
            this.ticketProperties.NoConfigurationRequired = true;
        }

        public void NoInstallationNeeded()
        {
            this.ticketProperties.NoInstallationRequired = true;
        }

        public void EstimateMeetingOrganized()
        {
            this.ticketProperties.EstimateMeetingOrganized = true;
        }

        public void EstimateMeetingPassed()
        {
            this.CurrentState = TicketStateRegistry.Instance.Get<EstimatedTicketState>();
            this.ticketProperties.EstimateMeetingOrganized = false;
        }

        public void EstimateMeetingFailed()
        {
            this.CurrentState = TicketStateRegistry.Instance.Get<EstimatingTicketState>();
            this.ticketProperties.EstimateMeetingOrganized = false;
        }

        public void Estimated()
        {
            this.CurrentState = TicketStateRegistry.Instance.Get<EstimatingMeetingTicketState>();
            this.ticketProperties.EstimateMeetingOrganized = false;
        }

        public void EstimateSpecificationReceived()
        {
            this.CurrentState = TicketStateRegistry.Instance.Get<EstimatingTicketState>();
        }

        public void ProgrammingSpecificationReceived()
        {
            this.CurrentState = TicketStateRegistry.Instance.Get<ProgrammingTicketState>();
        }

        public void BuildFinishedSuccessfully()
        {
            this.CurrentState = TicketStateRegistry.Instance.Get<TestingTicketState>();
            this.ticketProperties.SkipCodeReview = false;
        }

        public void BuildFailed()
        {
            this.ReopenToProgramming();
            this.ticketProperties.SkipCodeReview = false;
        }

        public void AddStatusUpdate(string authorAbbreviation, DateTime dateTime, string text)
        {
            this.TracTicket.StatusUpdates.Insert(0, new TicketStatusUpdate() { AuthorAbbreviation = authorAbbreviation, DateTime = dateTime, Text = text });
        }
    }
}
