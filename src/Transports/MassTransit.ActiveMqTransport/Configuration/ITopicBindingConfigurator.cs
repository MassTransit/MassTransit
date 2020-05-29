namespace MassTransit.ActiveMqTransport
{
    /// <summary>
    /// Used to configure the binding of an exchange (to either a queue or another exchange)
    /// </summary>
    public interface ITopicBindingConfigurator :
        ITopicConfigurator
    {
        /// <summary>
        /// A routing key for the exchange binding
        /// </summary>
        string Selector { set; }
    }
}
