namespace MassTransit
{
    public interface GrpcConsumeContext
    {
        string RoutingKey { get; }
    }
}
