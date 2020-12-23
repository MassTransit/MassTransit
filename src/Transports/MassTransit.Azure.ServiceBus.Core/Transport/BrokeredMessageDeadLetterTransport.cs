namespace MassTransit.Azure.ServiceBus.Core.Transport
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.Azure.ServiceBus;
    using Pipeline;
    using Transports;


    public class BrokeredMessageDeadLetterTransport :
        BrokeredMessageMoveTransport,
        IDeadLetterTransport
    {
        public BrokeredMessageDeadLetterTransport(IConnectionContextSupervisor supervisor, SendSettings settings)
            : base(supervisor, settings)
        {
        }

        public Task Send(ReceiveContext context, string reason)
        {
            void PreSend(Message message, IDictionary<string, object> headers)
            {
                headers.Set(new HeaderValue(MessageHeaders.Reason, reason ?? "Unspecified"));
            }

            return Move(context, PreSend);
        }
    }
}
