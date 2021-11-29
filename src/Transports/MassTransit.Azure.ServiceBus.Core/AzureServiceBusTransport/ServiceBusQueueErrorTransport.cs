namespace MassTransit.AzureServiceBusTransport
{
    using System.Collections.Generic;
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
            void PreSend(ServiceBusMessage message, IDictionary<string, object> headers)
            {
                headers.SetExceptionHeaders(context);

                message.TimeToLive = Defaults.BasicMessageTimeToLive;
            }

            return Move(context, PreSend);
        }
    }
}
