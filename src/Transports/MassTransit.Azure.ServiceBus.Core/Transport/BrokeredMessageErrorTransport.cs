namespace MassTransit.Azure.ServiceBus.Core.Transport
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using global::Azure.Messaging.ServiceBus;
    using Pipeline;
    using Transports;


    public class BrokeredMessageErrorTransport :
        BrokeredMessageMoveTransport,
        IErrorTransport
    {
        public BrokeredMessageErrorTransport(IConnectionContextSupervisor supervisor, SendSettings settings)
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
