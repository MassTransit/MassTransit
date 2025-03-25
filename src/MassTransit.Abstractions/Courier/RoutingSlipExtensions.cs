namespace MassTransit
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Courier;
    using Courier.Contracts;


    public static class RoutingSlipExtensions
    {
        /// <summary>
        /// Returns true if there are no remaining activities to be executed
        /// </summary>
        /// <param name="routingSlip"></param>
        /// <returns></returns>
        public static bool RanToCompletion(this RoutingSlip routingSlip)
        {
            return routingSlip.Itinerary.Count == 0;
        }

        public static Uri? GetNextExecuteAddress(this RoutingSlip routingSlip)
        {
            return routingSlip.Itinerary.Select(x => x.Address).First();
        }

        public static Uri? GetNextCompensateAddress(this RoutingSlip routingSlip)
        {
            return routingSlip.CompensateLogs.Select(x => x.Address).Last();
        }

        public static Task Execute<T>(this T source, RoutingSlip routingSlip)
            where T : IPublishEndpoint, ISendEndpointProvider
        {
            return new RoutingSlipExecutor(source, source).Execute(routingSlip, CancellationToken.None);
        }

        public static Task Execute<T>(this T source, RoutingSlip routingSlip, CancellationToken cancellationToken)
            where T : IPublishEndpoint, ISendEndpointProvider
        {
            return new RoutingSlipExecutor(source, source).Execute(routingSlip, cancellationToken);
        }

        /// <summary>
        /// Execute a routing slip via the <paramref name="sendEndpointProvider"/> and/or <paramref name="publishEndpoint"/> provided.
        /// This method works with the bus outbox (from the transactional outbox).
        /// </summary>
        /// <param name="routingSlip"></param>
        /// <param name="sendEndpointProvider"></param>
        /// <param name="publishEndpoint"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static Task Execute(this RoutingSlip routingSlip, ISendEndpointProvider sendEndpointProvider, IPublishEndpoint publishEndpoint,
            CancellationToken cancellationToken = default)
        {
            if (routingSlip == null)
                throw new ArgumentNullException(nameof(routingSlip));
            if (sendEndpointProvider == null)
                throw new ArgumentNullException(nameof(sendEndpointProvider));
            if (publishEndpoint == null)
                throw new ArgumentNullException(nameof(publishEndpoint));

            return new RoutingSlipExecutor(sendEndpointProvider, publishEndpoint).Execute(routingSlip, cancellationToken);
        }
    }
}
