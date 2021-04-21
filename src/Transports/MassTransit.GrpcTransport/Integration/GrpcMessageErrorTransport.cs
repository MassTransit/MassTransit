namespace MassTransit.GrpcTransport.Integration
{
    using System.Threading.Tasks;
    using Fabric;
    using Transports;


    public class GrpcMessageErrorTransport :
        GrpcMessageMoveTransport,
        IErrorTransport
    {
        public GrpcMessageErrorTransport(IGrpcExchange exchange)
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
