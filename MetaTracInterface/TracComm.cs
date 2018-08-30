using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using CookComputing.XmlRpc;
using MetaTracInterface.Helpers;
using MetaTracInterface.RPC;

namespace MetaTracInterface
{
    public class TracComm
    {
        private string username;
        private string password;
        private ITicketClient ticketClient;

        public TracComm(string username, string password)
        {
            this.username = username;
            this.password = password;

            this.ticketClient = XmlRpcProxyGen.Create<ITicketClient>();
            this.ticketClient.Credentials = new NetworkCredential(this.username, this.password);
        }

        public TracTicketData LoadTicket(int id)
        {
            object[] ticketData = this.ticketClient.GetTicket(id);
            object[] attachmentsData = this.ticketClient.GetAttachments(id);
            TracTicketData ticket = ParseTracTicket(ticketData);
            List<TicketAttachment> attachments = ParseAttachments(attachmentsData);
            ticket.Attachments = attachments;
            ticket.TestPlans = attachments.Where(attachment => attachment is TicketTestPlanAttachment).Cast<TicketTestPlanAttachment>().ToList();
            return ticket;
        }

        public TracTicketData UpdateTicket(IReadOnlyTracTicketData tracTicketData)
        {
            object[] ticketData = this.ticketClient.UpdateTicket(
                tracTicketData.ID,
                string.Empty,
                new
                {
                    action = "leave",
                    businessvalue = tracTicketData.BusinessValue.GetXMLRPCString(),
                    component = tracTicketData.Component.GetXMLRPCString(),
                    configsettings = tracTicketData.ConfigurationSettings.GetXMLRPCString(),
                    description = tracTicketData.Description.GetXMLRPCString(),
                    feature_branch = tracTicketData.FeatureBranch.GetXMLRPCString(),
                    howtoqa = tracTicketData.HowToQA.GetXMLRPCString(),
                    setupnotes = tracTicketData.InstallationNotes.GetXMLRPCString(),
                    milestone = tracTicketData.Milestone.GetXMLRPCString(),
                    owner = tracTicketData.Owner.GetXMLRPCString(),
                    parents = tracTicketData.ParentTicketID.GetXMLRPCString(),
                    priority = TracTypeConverters.TicketPriorityConverter.ConvertToSource(tracTicketData.Priority),
                    qaby = tracTicketData.QaBY.GetXMLRPCString(),
                    estimatedhours = tracTicketData.Remaining.GetXMLRPCString(),
                    reporter = tracTicketData.Reporter.GetXMLRPCString(),
                    sprintassignment = tracTicketData.SprintAssignment.GetXMLRPCString(),
                    sprintteam = tracTicketData.SprintTeam.GetXMLRPCString(),
                    status = TracTypeConverters.TicketStatusConverter.ConvertToSource(tracTicketData.Status),
                    statusupdatetext = TracTypeConverters.TicketStatusUpdatesConverter.ConvertToSource(tracTicketData.StatusUpdates),
                    summary = tracTicketData.Summary.GetXMLRPCString(),
                    targetversion = tracTicketData.TargetVersion.GetXMLRPCString(),
                    technotes = tracTicketData.TechnicalNotes.GetXMLRPCString(),
                    testplanreviewedprog = TracTypeConverters.BooleanTracConverter.ConvertToSource(tracTicketData.TestPlanReviewed),
                    totalhours = tracTicketData.TotalHours.GetXMLRPCString()
                });

            return ParseTracTicket(ticketData);
        }

        private static List<TicketAttachment> ParseAttachments(object[] attachmentsData)
        {
            List<TicketAttachment> attachments = new List<TicketAttachment>();
            foreach (object[] attachment in attachmentsData)
            {
                TicketAttachment ticketAttachment = new TicketAttachment()
                {
                    Author = (string)attachment[4],
                    Created = (DateTime)attachment[3],
                    Description = (string)attachment[1],
                    FileName = (string)attachment[0],
                    Size = (int)attachment[2]
                };

                TicketTestPlanAttachment testPlan = TicketTestPlanAttachment.TryParse(ticketAttachment);
                if (testPlan != null)
                {
                    attachments.Add(testPlan);
                }
                else
                {
                    attachments.Add(ticketAttachment);
                }
            }

            return attachments;
        }

        private static TracTicketData ParseTracTicket(object[] ticketData)
        {
            int ticketID = (int)ticketData[0];
            DateTime created = (DateTime)ticketData[1];
            DateTime updated = (DateTime)ticketData[2];
            XmlRpcStruct attributes = (XmlRpcStruct)ticketData[3];
            return new TracTicketData()
            {
                BusinessValue = attributes.GetValue<decimal?>("businessvalue"),
                Changed = updated,
                ChangeTime = attributes.GetValue<DateTime>("changetime").ToLocalTime(),
                Component = attributes.GetValue<string>("component"),
                ConfigurationSettings = attributes.GetValue<string>("configsettings"),
                Created = created,
                Description = attributes.GetValue<string>("description"),
                FeatureBranch = attributes.GetValue<string>("feature_branch"),
                HowToQA = attributes.GetValue<string>("howtoqa"),
                ID = ticketID,
                InstallationNotes = attributes.GetValue<string>("setupnotes"),
                Milestone = attributes.GetValue<string>("milestone"),
                Owner = attributes.GetValue<string>("owner"),
                ParentTicketID = attributes.GetValue<int?>("parents"),
                Priority = TracTypeConverters.TicketPriorityConverter.ConvertToTarget(attributes.GetValue<string>("priority")),
                QaBY = attributes.GetValue<string>("qaby"),
                Remaining = attributes.GetValue<decimal>("estimatedhours"),
                Reporter = attributes.GetValue<string>("reporter"),
                SprintAssignment = attributes.GetValue<string>("sprintassignment"),
                SprintTeam = attributes.GetValue<string>("sprintteam"),
                Status = TracTypeConverters.TicketStatusConverter.ConvertToTarget(attributes.GetValue<string>("status")),
                StatusUpdates = TracTypeConverters.TicketStatusUpdatesConverter.ConvertToTarget(attributes.GetValue<string>("statusupdatetext")),
                Summary = attributes.GetValue<string>("summary"),
                TargetVersion = attributes.GetValue<string>("targetversion"),
                TechnicalNotes = attributes.GetValue<string>("technotes"),
                TestPlanReviewed = TracTypeConverters.BooleanTracConverter.ConvertToTarget(attributes.GetValue<string>("testplanreviewedprog")),
                TotalHours = attributes.GetValue<decimal>("totalhours")
            };
        }
    }
}
