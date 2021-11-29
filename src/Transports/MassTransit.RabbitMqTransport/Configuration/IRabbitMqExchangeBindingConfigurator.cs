namespace MassTransit
{
    /// <summary>
    /// Used to configure the binding of an exchange (to either a queue or another exchange)
    /// </summary>
    public interface IRabbitMqExchangeBindingConfigurator :
        IRabbitMqExchangeConfigurator
    {
        /// <summary>
        /// A routing key for the exchange binding
        /// </summary>
        string RoutingKey { set; }

        /// <summary>
        /// Sets the binding argument, or removes it if value is null
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        void SetBindingArgument(string key, object value);
    }
}
