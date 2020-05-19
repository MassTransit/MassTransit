namespace MassTransit.ActiveMqTransport
{
    public interface IQueueBindingConfigurator :
        IQueueConfigurator
    {
        /// <summary>
        /// A routing key for the exchange binding
        /// </summary>
        string Selector { set; }
    }
}
