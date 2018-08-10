using ClassLibrary.GenericRegistry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobLogger.Tickets.States
{
    public class TicketStateRegistry
    {
        private static TicketStateRegistry instance;

        private readonly GenericRegistry<TicketState> registry = new GenericRegistry<TicketState>();

        private TicketStateRegistry()
        {
            this.registry.RegisterWithType(new ProgrammingTicketState());
            this.registry.RegisterWithType(new CodeReviewTicketState());
            this.registry.RegisterWithType(new EstimatedTicketState());
            this.registry.RegisterWithType(new EstimatingMeetingTicketState());
            this.registry.RegisterWithType(new EstimatingTicketState());
            this.registry.RegisterWithType(new InternalCodeReviewTicketState());
            this.registry.RegisterWithType(new MergedTicketState());
            this.registry.RegisterWithType(new TestingTicketState());
            this.registry.RegisterWithType(new WaitingForProgrammingSpecificationTicketState());
            this.registry.RegisterWithType(new DoneTicketState());
        }

        public static TicketStateRegistry Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new TicketStateRegistry();
                }

                return instance;
            }
        }

        public TTicketState Get<TTicketState>()
            where TTicketState : TicketState
        {
            return this.registry.GetByType<TTicketState>();
        }

        public TicketState GetByCode(string code)
        {
            foreach(TicketState state in this.registry.GetItems())
            {
                if(state.Code.Equals(code, StringComparison.OrdinalIgnoreCase))
                {
                    return state;
                }
            }

            return null;
        }
    }
}
