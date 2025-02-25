namespace MassTransit.AzureServiceBusTransport
{
    using System.Threading.Tasks;
    using Azure.Messaging.ServiceBus;
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
            void PreSend(ServiceBusMessage message, SendHeaders headers)
            {
                headers.Set(MessageHeaders.Reason, reason ?? "Unspecified");
            }

            return Move(context, PreSend);
        }
    }
}
