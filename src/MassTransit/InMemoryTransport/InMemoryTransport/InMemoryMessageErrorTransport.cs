namespace MassTransit.InMemoryTransport
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Fabric;
    using Transports;


    public class InMemoryMessageErrorTransport :
        InMemoryMessageMoveTransport,
        IErrorTransport
    {
        public InMemoryMessageErrorTransport(IInMemoryExchange exchange)
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
