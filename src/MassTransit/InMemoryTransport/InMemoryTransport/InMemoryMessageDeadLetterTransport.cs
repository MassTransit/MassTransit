namespace MassTransit.InMemoryTransport
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Transports;
    using Transports.Fabric;


    public class InMemoryMessageDeadLetterTransport :
        InMemoryMessageMoveTransport,
        IDeadLetterTransport
    {
        public InMemoryMessageDeadLetterTransport(IMessageExchange<InMemoryTransportMessage> exchange)
            : base(exchange)
        {
        }

        public Task Send(ReceiveContext context, string reason)
        {
            void PreSend(InMemoryTransportMessage message, IDictionary<string, object> headers)
            {
                headers.Set(new HeaderValue(MessageHeaders.Reason, reason ?? "Unspecified"));
            }

            return Move(context, PreSend);
        }
    }
}
