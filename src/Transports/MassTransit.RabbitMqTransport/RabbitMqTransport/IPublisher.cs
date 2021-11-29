namespace MassTransit.RabbitMqTransport
{
    using System;
    using System.Threading.Tasks;
    using RabbitMQ.Client;


    public interface IPublisher :
        IAsyncDisposable
    {
        /// <summary>
        /// Publish the message to RabbitMQ. The returned Task is completed depending upon the configuration of the
        /// channel/connection. If publisher confirmation is disable, and batch is disabled, the Task is completed
        /// immediately. If publisher confirmation is enabled, the Task is completed once
        /// the broker has confirmed the message. If publisher confirmation is disabled, but batch is enabled, the
        /// task is completed once the batch has been sent to the broker.
        /// </summary>
        /// <param name="exchange"></param>
        /// <param name="routingKey"></param>
        /// <param name="mandatory"></param>
        /// <param name="basicProperties"></param>
        /// <param name="body"></param>
        /// <param name="awaitAck">
        /// Complete the Task only once the message has been confirmed, if confirmation is enabled
        /// </param>
        /// <returns></returns>
        Task Publish(string exchange, string routingKey, bool mandatory, IBasicProperties basicProperties, byte[] body, bool awaitAck);
    }
}
