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
            this.tracComm = new TracComm(username, password);
            this.stateQueues = stateQueues;
        }

        public Ticket CreateNew(int id, StateQueue stateQueue)
        {
            IReadonlyTracTicket tracTicket = this.tracComm.LoadTicket(id);
            return new Ticket(tracTicket, stateQueue.First(), new CustomTicketProperties(), stateQueue);
        }

        public void ReloadTracTicket(Ticket ticket)
        {
            ticket.TracTicket = this.tracComm.LoadTicket(ticket.TracTicket.ID);
        }

        public IEnumerable<Ticket> Load()
        {
            List<TicketSerializableData> serializableDataList = JsonConvert.DeserializeObject<List<TicketSerializableData>>(File.ReadAllText(this.filePath));
            List<Ticket> tickets = new List<Ticket>();
            foreach (TicketSerializableData data in serializableDataList)
            {
                IReadonlyTracTicket tracTicket = this.tracComm.LoadTicket(data.ID);
                TicketState ticketState = null;
                StateQueue stateQueue = null;
                foreach (StateQueue queue in this.stateQueues)
                {
                    ticketState = queue.FindTicketState(data.StatusCode);
                    if (ticketState != null)
                    {
                        stateQueue = queue;
                        break;
                    }
                }

                if (ticketState != null && stateQueue != null)
                {
                    tickets.Add(new Ticket(tracTicket, ticketState, data.TicketProperties, stateQueue));
                }
            }

            return tickets;
        }

        public void Save(IEnumerable<Ticket> tickets)
        {
            List<TicketSerializableData> serializableDataList = new List<TicketSerializableData>();
            foreach (Ticket ticket in tickets)
            {
                serializableDataList.Add(new TicketSerializableData()
                {
                    ID = ticket.TracTicket.ID,
                    StatusCode = ticket.CurrentState.Code,
                    TicketProperties = ticket.TicketProperties
                });
            }

            File.WriteAllText(this.filePath, JsonConvert.SerializeObject(serializableDataList));
        }
    }
}
