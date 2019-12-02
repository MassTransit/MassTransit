namespace MassTransit.AzureServiceBusTransport.Transport
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.ServiceBus.Messaging;
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
            void PreSend(BrokeredMessage message, IDictionary<string, object> headers)
            {
                headers.Set(new HeaderValue(MessageHeaders.Reason, reason ?? "Unspecified"));
            }

            return Move(context, PreSend);
        }
    }
}
