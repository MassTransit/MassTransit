namespace MassTransit.GrpcTransport
{
    public class EmptyRoutingKeyFormatter :
        IRoutingKeyFormatter
    {
        string IRoutingKeyFormatter.FormatRoutingKey<T>(GrpcSendContext<T> context)
        {
            return context.RoutingKey;
        }
    }
}
