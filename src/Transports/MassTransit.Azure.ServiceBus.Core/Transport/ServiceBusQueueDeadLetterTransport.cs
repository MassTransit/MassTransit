namespace MassTransit.Azure.ServiceBus.Core.Transport
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using global::Azure.Messaging.ServiceBus;
    using Pipeline;
    using Transports;


    public class ServiceBusQueueDeadLetterTransport :
        ServiceBusQueueMoveTransport,
        IDeadLetterTransport
    {
        public ServiceBusQueueDeadLetterTransport(IConnectionContextSupervisor supervisor, SendSettings settings)
            : base(supervisor, settings)
        {
        }

        public Task Send(ReceiveContext context, string reason)
        {
            void PreSend(ServiceBusMessage message, IDictionary<string, object> headers)
            {
                headers.Set(new HeaderValue(MessageHeaders.Reason, reason ?? "Unspecified"));
            }

            return Move(context, PreSend);
        }
    }
}
