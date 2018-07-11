using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CookComputing.XmlRpc;

namespace MetaTracInterface.RPC
{
    [XmlRpcUrl("http://10.71.23.133:8088/Malo/login/xmlrpc")]
    public interface ITicketClient : IXmlRpcProxy
    {
        [XmlRpcMethod("ticket.get")]
        object[] GetTicket(int id);

        [XmlRpcMethod("ticket.listAttachments")]
        object[] GetAttachments(int id);

        [XmlRpcMethod("ticket.getActions")]
        object[] GetActions(int id);

        [XmlRpcMethod("ticket.update")]
        object[] UpdateTicket(int id, string comment, object attributes);
    }
}
