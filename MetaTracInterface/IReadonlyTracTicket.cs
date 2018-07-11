using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetaTracInterface
{
    public interface IReadonlyTracTicket
    {
        int ID { get; }
        DateTime Created { get; }
        DateTime Changed { get; }
        DateTime ChangeTime { get; }
        decimal Remaining { get; }
        List<TicketStatusUpdate> StatusUpdates { get; }
        string FeatureBranch { get; }
        int? ParentTicketID { get; }
        TicketPriority Priority { get; }
        TicketStatus Status { get; }
        decimal TotalHours { get; }
        string Description { get; }
        string TargetVersion { get; }
        string Milestone { get; }
        string SprintAssignment { get; }
        string HowToQA { get; }
        string SprintTeam { get; }
        string Summary { get; }
        decimal? BusinessValue { get; }
        string Reporter { get; }
        string Owner { get; }
        string Component { get; }
        bool TestPlanReviewed { get; }
        string TechnicalNotes { get; }
        string ConfigurationSettings { get; }
        string InstallationNotes { get; }
        string QaBY { get; }
        IEnumerable<TicketAttachment> Attachments { get;  }
        IEnumerable<TicketTestPlanAttachment> TestPlans { get;  }
    }
}
