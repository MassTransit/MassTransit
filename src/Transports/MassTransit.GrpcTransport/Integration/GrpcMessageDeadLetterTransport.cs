namespace MassTransit.GrpcTransport.Integration
{
    using System.Threading.Tasks;
    using Fabric;
    using Transports;


    public class GrpcMessageDeadLetterTransport :
        GrpcMessageMoveTransport,
        IDeadLetterTransport
    {
        public GrpcMessageDeadLetterTransport(IGrpcExchange exchange)
            : base(exchange)
        {
        }

        public Task Send(ReceiveContext context, string reason)
        {
            void PreSend(GrpcTransportMessage message, SendHeaders headers)
            {
                headers.Set(MessageHeaders.Reason, reason ?? "Unspecified");
            }

            return Move(context, PreSend);
        }
    }
}
