namespace MassTransit
{
    using Transports.Fabric;


    public interface IGrpcReceiveEndpointConfigurator :
        IReceiveEndpointConfigurator
    {
        /// <summary>
        /// Bind an exchange to the receive endpoint queue
        /// </summary>
        /// <param name="exchangeName">The exchange name (not case-sensitive)</param>
        /// <param name="exchangeType">The exchange type</param>
        /// <param name="routingKey">Only valid for direct/topic exchanges</param>
        void Bind(string exchangeName, ExchangeType exchangeType = ExchangeType.FanOut, string routingKey = default);

        /// <summary>
        /// Bind an exchange to the receive endpoint queue
        /// </summary>
        /// <param name="exchangeType">The exchange type</param>
        /// <param name="routingKey">Only valid for direct/topic exchanges</param>
        void Bind<T>(ExchangeType? exchangeType = default, string routingKey = default)
            where T : class;
    }
}
