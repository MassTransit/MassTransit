namespace MassTransit.ActiveMqTransport
{
    /// <summary>
    /// Configures a queue/exchange pair in ActiveMQ
    /// </summary>
    public interface IQueueConfigurator
    {
        /// <summary>
        /// Specify the queue should be durable (survives broker restart) or in-memory
        /// </summary>
        /// <value>True for a durable queue, False for an in-memory queue</value>
        bool Durable { set; }

        /// <summary>
        /// Specify that the queue (and the exchange of the same name) should be created as auto-delete
        /// </summary>
        bool AutoDelete { set; }
    }
}
