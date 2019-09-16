namespace MassTransit.Azure.ServiceBus.Core.Transport
{
    using System.Threading.Tasks;
    using Microsoft.Azure.ServiceBus;
    using Pipeline;
    using Transports;


    public class BrokeredMessageDeadLetterTransport :
        BrokeredMessageMoveTransport,
        IDeadLetterTransport
    {
        public BrokeredMessageDeadLetterTransport(ISendEndpointContextSupervisor supervisor)
            : base(supervisor)
        {
        }

        public Task Send(ReceiveContext context, string reason)
        {
            void PreSend(Message message, SendHeaders headers)
            {
                headers.Set(MessageHeaders.Reason, reason ?? "Unspecified");
            }

            return Move(context, PreSend);
        }
    }
}
