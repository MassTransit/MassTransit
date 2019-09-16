namespace MassTransit.Azure.ServiceBus.Core.Transport
{
    using System.Threading.Tasks;
    using Microsoft.Azure.ServiceBus;
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
            void PreSend(Message message, SendHeaders headers)
            {
                headers.SetExceptionHeaders(context);

                message.TimeToLive = Defaults.BasicMessageTimeToLive;
            }

            return Move(context, PreSend);
        }
    }
}
