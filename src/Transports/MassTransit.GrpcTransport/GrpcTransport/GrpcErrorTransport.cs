namespace MassTransit.GrpcTransport
{
    using System.Threading.Tasks;
    using Fabric;
    using Transports;


    public class GrpcErrorTransport :
        GrpcMoveTransport,
        IErrorTransport
    {
        public GrpcErrorTransport(IMessageExchange exchange)
            : base(exchange)
        {
        }

        public Task Send(ExceptionReceiveContext context)
        {
            void PreSend(GrpcTransportMessage message, SendHeaders headers)
            {
                headers.SetExceptionHeaders(context);
            }

            return Move(context, PreSend);
        }
    }
}
