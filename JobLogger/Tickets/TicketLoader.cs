using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JobLogger.Tickets.States;
using MetaTracInterface;
using Newtonsoft.Json;

namespace JobLogger.Tickets
{
    class TicketLoader
    {
        private string filePath;
        private TracComm tracComm;
        private StateQueues stateQueues;

        public TicketLoader(string filePath, string username, string password, StateQueues stateQueues)
        {
            this.filePath = filePath;
            if (!File.Exists(filePath))
            {
                this.Save(new List<Ticket>(), false);
            }

            this.tracComm = new TracComm(username, password);

            this.stateQueues = stateQueues;
        }

        public Ticket CreateNew(int id, TicketState initialState)
        {
            TracTicketData tracTicket = this.tracComm.LoadTicket(id);
            return new Ticket(tracTicket, initialState, new CustomTicketProperties());
        }

        public IEnumerable<Ticket> Load(bool includeDone)
        {
            List<TicketSerializableData> serializableDataList = JsonConvert.DeserializeObject<List<TicketSerializableData>>(File.ReadAllText(this.filePath));
            foreach (TicketSerializableData data in serializableDataList.OrderBy(serializableTicketData => serializableTicketData.ID))
            {
                TicketState ticketState = TicketStateRegistry.Instance.GetByCode(data.StatusCode);
                if (ticketState != null && (ticketState != TicketStateRegistry.Instance.Get<DoneTicketState>() || includeDone))
                {
                    TracTicketData tracTicket = this.tracComm.LoadTicket(data.ID);
                    Ticket ticket = new Ticket(tracTicket, ticketState, data.TicketProperties);
                    yield return ticket;
                }
            }
        }

        public void Save(IEnumerable<Ticket> tickets, bool includeOldDone)
        {
            string doneTicketCode = TicketStateRegistry.Instance.Get<DoneTicketState>().Code;
            List<TicketSerializableData> serializableDataList = new List<TicketSerializableData>();
            if (includeOldDone)
            {
                serializableDataList.AddRange(JsonConvert.DeserializeObject<List<TicketSerializableData>>(File.ReadAllText(this.filePath)).Where(serializableTicket => serializableTicket.StatusCode == doneTicketCode));
            }

            foreach (Ticket ticket in tickets)
            {
                serializableDataList.Add(new TicketSerializableData()
                {
                    ID = ticket.TracTicket.ID,
                    StatusCode = ticket.CurrentState.Code,
                    TicketProperties = CustomTicketProperties.CloneFrom(ticket.TicketProperties)
                });
            }

            File.WriteAllText(this.filePath, JsonConvert.SerializeObject(serializableDataList, Newtonsoft.Json.Formatting.Indented));
        }

        public void SaveTracTicket(IReadOnlyTracTicketData tracTicket)
        {
            this.tracComm.UpdateTicket(tracTicket);
        }
    }
}
