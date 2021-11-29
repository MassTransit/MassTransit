namespace MassTransit.GrpcTransport
{
    using System.Threading;
    using Context;


    public class TransportGrpcSendContext<T> :
        MessageSendContext<T>,
        GrpcSendContext<T>
        where T : class
    {
        public TransportGrpcSendContext(string exchange, T message, CancellationToken cancellationToken)
            : base(message, cancellationToken)
        {
            Exchange = exchange;
            RoutingKey = default;
        }

        public string Exchange { get; }
        public string RoutingKey { get; set; }
    }
}
