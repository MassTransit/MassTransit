namespace MassTransit
{
    using ActiveMqTransport;


    /// <summary>
    /// Used to configure the binding of an exchange (to either a queue or another exchange)
    /// </summary>
    public interface IActiveMqTopicBindingConfigurator :
        IActiveMqTopicConfigurator
    {
        /// <summary>
        /// A routing key for the exchange binding
        /// </summary>
        string Selector { set; }
    }
}
