namespace MassTransit.InMemoryTransport
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Transports;
    using Transports.Fabric;


    public class InMemoryMessageErrorTransport :
        InMemoryMessageMoveTransport,
        IErrorTransport
    {
        public InMemoryMessageErrorTransport(IMessageExchange<InMemoryTransportMessage> exchange)
            : base(exchange)
        {
        }

        public Task Send(ExceptionReceiveContext context)
        {
            void PreSend(InMemoryTransportMessage message, IDictionary<string, object> headers)
            {
                headers.SetExceptionHeaders(context);
            }

            return Move(context, PreSend);
        }
    }
}
