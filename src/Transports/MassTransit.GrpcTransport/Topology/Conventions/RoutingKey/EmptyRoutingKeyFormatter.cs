namespace MassTransit.GrpcTransport.Topology.Conventions.RoutingKey
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
