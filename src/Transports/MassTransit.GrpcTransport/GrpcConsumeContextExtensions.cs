namespace MassTransit
{
    using System;


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

        /// <summary>
        /// Sets the routing key for this message
        /// </summary>
        /// <param name="context"></param>
        /// <param name="routingKey">The routing key for this message</param>
        public static void SetRoutingKey(this SendContext context, string routingKey)
        {
            if (!context.TryGetPayload(out GrpcSendContext sendContext))
                throw new ArgumentException("The GrpcSendContext was not available");

            sendContext.RoutingKey = routingKey;
        }

        /// <summary>
        /// Sets the routing key for this message
        /// </summary>
        /// <param name="context"></param>
        /// <param name="routingKey">The routing key for this message</param>
        public static bool TrySetRoutingKey(this SendContext context, string routingKey)
        {
            if (!context.TryGetPayload(out GrpcSendContext sendContext))
                return false;

            sendContext.RoutingKey = routingKey;
            return true;
        }
    }
}
