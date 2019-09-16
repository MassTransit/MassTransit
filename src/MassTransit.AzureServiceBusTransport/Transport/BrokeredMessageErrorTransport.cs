namespace MassTransit.AzureServiceBusTransport.Transport
{
    using System.Threading.Tasks;
    using Microsoft.ServiceBus.Messaging;
    using Pipeline;
    using Transports;


    public class BrokeredMessageErrorTransport :
        BrokeredMessageMoveTransport,
        IErrorTransport
    {
        public BrokeredMessageErrorTransport(ISendEndpointContextSupervisor supervisor)
            : base(supervisor)
        {
        }

        public Task Send(ExceptionReceiveContext context)
        {
            void PreSend(BrokeredMessage message, SendHeaders headers)
            {
                headers.SetExceptionHeaders(context);

                message.TimeToLive = Defaults.BasicMessageTimeToLive;
            }

            return Move(context, PreSend);
        }
    }
}
