namespace MassTransit
{
    using Transports.Fabric;


    public static class GrpcReceiveEndpointConfiguratorExtensions
    {
        /// <summary>
        /// Bind an exchange to the receive endpoint queue
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="exchangeName">The exchange name (not case-sensitive)</param>
        /// <param name="exchangeType">The exchange type</param>
        /// <param name="routingKey">Only valid for direct/topic exchanges</param>
        public static void Bind(this IReceiveEndpointConfigurator configurator, string exchangeName, ExchangeType exchangeType = ExchangeType.FanOut,
            string routingKey = default)
        {
            if (configurator is IGrpcReceiveEndpointConfigurator grpc)
                grpc.Bind(exchangeName, exchangeType, routingKey);
        }

        /// <summary>
        /// Bind an exchange to the receive endpoint queue
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="exchangeType">The exchange type</param>
        /// <param name="routingKey">Only valid for direct/topic exchanges</param>
        public static void Bind<T>(this IReceiveEndpointConfigurator configurator, ExchangeType? exchangeType = default, string routingKey = default)
            where T : class
        {
            if (configurator is IGrpcReceiveEndpointConfigurator grpc)
                grpc.Bind<T>(exchangeType, routingKey);
        }
    }
}
