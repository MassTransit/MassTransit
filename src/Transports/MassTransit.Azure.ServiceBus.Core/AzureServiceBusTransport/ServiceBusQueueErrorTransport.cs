namespace MassTransit.AzureServiceBusTransport
{
    using System.Threading.Tasks;
    using Azure.Messaging.ServiceBus;
    using Transports;


    public class ServiceBusQueueErrorTransport :
        ServiceBusQueueMoveTransport,
        IErrorTransport
    {
        public ServiceBusQueueErrorTransport(IConnectionContextSupervisor supervisor, SendSettings settings)
            : base(supervisor, settings)
        {
        }

        public Task Send(ExceptionReceiveContext context)
        {
            void PreSend(ServiceBusMessage message, SendHeaders headers)
            {
                headers.CopyFrom(context.ExceptionHeaders);

                message.TimeToLive = Defaults.BasicMessageTimeToLive;
            }

            return Move(context, PreSend);
        }
    }
}
