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

        public void LoadTicketData(TracTicket tracTicket)
        {
            object[] ticketData = this.ticketClient.GetTicket(tracTicket.ID);
            object[] attachmentsData = this.ticketClient.GetAttachments(tracTicket.ID);
            ParseTracTicket(tracTicket, ticketData);
            List<TicketAttachment> attachments = ParseAttachments(attachmentsData);
            tracTicket.Attachments = attachments;
            tracTicket.TestPlans = attachments.Where(attachment => attachment is TicketTestPlanAttachment).Cast<TicketTestPlanAttachment>().ToList();
        }

        public void UpdateTicket(TracTicket tracTicket)
        {
            object[] ticketData = this.ticketClient.UpdateTicket(
                tracTicket.ID,
                string.Empty,
                new
                {
                    action = "leave",
                    businessvalue = tracTicket.BusinessValue.GetXMLRPCString(),
                    component = tracTicket.Component.GetXMLRPCString(),
                    configsettings = tracTicket.ConfigurationSettings.GetXMLRPCString(),
                    description = tracTicket.Description.GetXMLRPCString(),
                    feature_branch = tracTicket.FeatureBranch.GetXMLRPCString(),
                    howtoqa = tracTicket.HowToQA.GetXMLRPCString(),
                    setupnotes = tracTicket.InstallationNotes.GetXMLRPCString(),
                    milestone = tracTicket.Milestone.GetXMLRPCString(),
                    owner = tracTicket.Owner.GetXMLRPCString(),
                    parents = tracTicket.ParentTicketID.GetXMLRPCString(),
                    priority = TracTypeConverters.TicketPriorityConverter.ConvertToSource(tracTicket.Priority),
                    qaby = tracTicket.QaBY.GetXMLRPCString(),
                    estimatedhours = tracTicket.Remaining.GetXMLRPCString(),
                    reporter = tracTicket.Reporter.GetXMLRPCString(),
                    sprintassignment = tracTicket.SprintAssignment.GetXMLRPCString(),
                    sprintteam = tracTicket.SprintTeam.GetXMLRPCString(),
                    status = TracTypeConverters.TicketStatusConverter.ConvertToSource(tracTicket.Status),
                    statusupdatetext = TracTypeConverters.TicketStatusUpdatesConverter.ConvertToSource(tracTicket.StatusUpdates),
                    summary = tracTicket.Summary.GetXMLRPCString(),
                    targetversion = tracTicket.TargetVersion.GetXMLRPCString(),
                    technotes = tracTicket.TechnicalNotes.GetXMLRPCString(),
                    testplanreviewedprog = TracTypeConverters.BooleanTracConverter.ConvertToSource(tracTicket.TestPlanReviewed),
                    totalhours = tracTicket.TotalHours.GetXMLRPCString()
                });

            ParseTracTicket(tracTicket, ticketData);
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

        private static void ParseTracTicket(TracTicket targetTicket, object[] ticketData)
        {
            int ticketID = (int)ticketData[0];
            DateTime created = (DateTime)ticketData[1];
            DateTime updated = (DateTime)ticketData[2];
            XmlRpcStruct attributes = (XmlRpcStruct)ticketData[3];
            targetTicket.BusinessValue = attributes.GetValue<decimal?>("businessvalue");
            targetTicket.Changed = updated;
            targetTicket.ChangeTime = attributes.GetValue<DateTime>("changetime").ToLocalTime();
            targetTicket.Component = attributes.GetValue<string>("component");
            targetTicket.ConfigurationSettings = attributes.GetValue<string>("configsettings");
            targetTicket.Created = created;
            targetTicket.Description = attributes.GetValue<string>("description");
            targetTicket.FeatureBranch = attributes.GetValue<string>("feature_branch");
            targetTicket.HowToQA = attributes.GetValue<string>("howtoqa");
            targetTicket.InstallationNotes = attributes.GetValue<string>("setupnotes");
            targetTicket.Milestone = attributes.GetValue<string>("milestone");
            targetTicket.Owner = attributes.GetValue<string>("owner");
            targetTicket.ParentTicketID = attributes.GetValue<int?>("parents");
            targetTicket.Priority = TracTypeConverters.TicketPriorityConverter.ConvertToTarget(attributes.GetValue<string>("priority"));
            targetTicket.QaBY = attributes.GetValue<string>("qaby");
            targetTicket.Remaining = attributes.GetValue<decimal>("estimatedhours");
            targetTicket.Reporter = attributes.GetValue<string>("reporter");
            targetTicket.SprintAssignment = attributes.GetValue<string>("sprintassignment");
            targetTicket.SprintTeam = attributes.GetValue<string>("sprintteam");
            targetTicket.Status = TracTypeConverters.TicketStatusConverter.ConvertToTarget(attributes.GetValue<string>("status"));
            targetTicket.StatusUpdates = TracTypeConverters.TicketStatusUpdatesConverter.ConvertToTarget(attributes.GetValue<string>("statusupdatetext"));
            targetTicket.Summary = attributes.GetValue<string>("summary");
            targetTicket.TargetVersion = attributes.GetValue<string>("targetversion");
            targetTicket.TechnicalNotes = attributes.GetValue<string>("technotes");
            targetTicket.TestPlanReviewed = TracTypeConverters.BooleanTracConverter.ConvertToTarget(attributes.GetValue<string>("testplanreviewedprog"));
            targetTicket.TotalHours = attributes.GetValue<decimal>("totalhours");
        }
    }
}
