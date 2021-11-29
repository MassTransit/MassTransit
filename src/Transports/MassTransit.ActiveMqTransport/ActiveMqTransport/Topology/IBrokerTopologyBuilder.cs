namespace MassTransit.ActiveMqTransport.Topology
{
    public interface IBrokerTopologyBuilder
    {
        /// <summary>
        /// Declares an exchange
        /// </summary>
        /// <param name="name">The exchange name</param>
        /// <param name="durable">A durable exchange survives a broker restart</param>
        /// <param name="autoDelete">Automatically delete if the broker connection is closed</param>
        /// <returns>An entity handle used to reference the exchange in subsequent calls</returns>
        TopicHandle CreateTopic(string name, bool durable, bool autoDelete);

        /// <summary>
        /// Declares a queue
        /// </summary>
        /// <param name="name"></param>
        /// <param name="autoDelete"></param>
        /// <returns></returns>
        QueueHandle CreateQueue(string name, bool autoDelete);

        /// <summary>
        /// Binds an exchange to a queue, with the specified routing key and arguments
        /// </summary>
        /// <param name="topic"></param>
        /// <param name="queue"></param>
        /// <param name="selector"></param>
        /// <returns></returns>
        ConsumerHandle BindConsumer(TopicHandle topic, QueueHandle queue, string selector);
    }
}
