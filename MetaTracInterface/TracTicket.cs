using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetaTracInterface
{
    public class TracTicket : IReadonlyTracTicket
    {
        public int ID { get; set; }
        public DateTime Created { get; set; }
        public DateTime Changed { get; set; }
        public DateTime ChangeTime { get; set; }
        public decimal Remaining { get; set; }
        public List<TicketStatusUpdate> StatusUpdates { get; set; }
        public string FeatureBranch { get; set; }
        public int? ParentTicketID { get; set; }
        public TicketPriority Priority { get; set; }
        public TicketStatus Status { get; set; }
        public decimal TotalHours { get; set; }
        public string Description { get; set; }
        public string TargetVersion { get; set; }
        public string Milestone { get; set; }
        public string SprintAssignment { get; set; }
        public string HowToQA { get; set; }
        public string SprintTeam { get; set; }
        public string Summary { get; set; }
        public decimal? BusinessValue { get; set; }
        public string Reporter { get; set; }
        public string Owner { get; set; }
        public string Component { get; set; }
        public bool TestPlanReviewed { get; set; }
        public string TechnicalNotes { get; set; }
        public string ConfigurationSettings { get; set; }
        public string InstallationNotes { get; set; }
        public string QaBY { get; set; }
        public List<TicketAttachment> Attachments { get; set; }
        public List<TicketTestPlanAttachment> TestPlans { get; set; }
        IEnumerable<TicketAttachment> IReadonlyTracTicket.Attachments { get { return this.Attachments; } }
        IEnumerable<TicketTestPlanAttachment> IReadonlyTracTicket.TestPlans { get { return this.TestPlans; } }
    }
}
