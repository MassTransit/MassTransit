namespace MassTransit.RabbitMqTransport.Topology
{
    using System.Collections.Generic;


    public interface IBrokerTopologyBuilder
    {
        /// <summary>
        /// Declares an exchange
        /// </summary>
        /// <param name="name">The exchange name</param>
        /// <param name="type">The exchange type</param>
        /// <param name="durable">A durable exchange survives a broker restart</param>
        /// <param name="autoDelete">Automatically delete if the broker connection is closed</param>
        /// <param name="arguments">The exchange arguments</param>
        /// <returns>An entity handle used to reference the exchange in subsequent calls</returns>
        ExchangeHandle ExchangeDeclare(string name, string type, bool durable, bool autoDelete, IDictionary<string, object> arguments);

        /// <summary>
        /// Bind an exchange to an exchange, with the specified routing key and arguments
        /// </summary>
        /// <param name="source">The source exchange</param>
        /// <param name="destination">The destination exchange</param>
        /// <param name="routingKey">The binding routing key</param>
        /// <param name="arguments">The binging arguments</param>
        /// <returns>An entity handle used to reference the binding in subsequent calls</returns>
        ExchangeBindingHandle ExchangeBind(ExchangeHandle source, ExchangeHandle destination, string routingKey, IDictionary<string, object> arguments);

        /// <summary>
        /// Declares a queue
        /// </summary>
        /// <param name="name"></param>
        /// <param name="durable"></param>
        /// <param name="autoDelete"></param>
        /// <param name="exclusive"></param>
        /// <param name="arguments"></param>
        /// <returns></returns>
        QueueHandle QueueDeclare(string name, bool durable, bool autoDelete, bool exclusive, IDictionary<string, object> arguments);

        /// <summary>
        /// Binds an exchange to a queue, with the specified routing key and arguments
        /// </summary>
        /// <param name="exchange"></param>
        /// <param name="queue"></param>
        /// <param name="routingKey"></param>
        /// <param name="arguments"></param>
        /// <returns></returns>
        QueueBindingHandle QueueBind(ExchangeHandle exchange, QueueHandle queue, string routingKey, IDictionary<string, object> arguments);
    }
}
