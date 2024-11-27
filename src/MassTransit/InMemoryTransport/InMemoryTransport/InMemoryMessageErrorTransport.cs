namespace MassTransit.InMemoryTransport
{
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
            void PreSend(InMemoryTransportMessage message, SendHeaders headers)
            {
                headers.CopyFrom(context.ExceptionHeaders);
            }

            return Move(context, PreSend);
        }
    }
}
