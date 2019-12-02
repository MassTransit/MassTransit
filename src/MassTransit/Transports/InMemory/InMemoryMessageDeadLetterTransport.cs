namespace MassTransit.Transports.InMemory
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Fabric;


    public class InMemoryMessageDeadLetterTransport :
        InMemoryMessageMoveTransport,
        IDeadLetterTransport
    {
        public InMemoryMessageDeadLetterTransport(IInMemoryExchange exchange)
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
