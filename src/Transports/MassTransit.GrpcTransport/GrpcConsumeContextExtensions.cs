namespace MassTransit
{
    using GrpcTransport;
    using GrpcTransport.Contexts;


    public static class GrpcConsumeContextExtensions
    {
        public static string RoutingKey(this ConsumeContext context)
        {
            return context.TryGetPayload<GrpcConsumeContext>(out var consumeContext) ? consumeContext.RoutingKey : string.Empty;
        }

        public static string RoutingKey(this SendContext context)
        {
            return context.TryGetPayload<GrpcSendContext>(out var sendContext) ? sendContext.RoutingKey : string.Empty;
        }
    }
}
