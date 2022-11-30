namespace MassTransit
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Azure.Messaging.ServiceBus;


    /// <summary>
    /// Called by an Azure Function to handle messages using configured consumers, sagas, or activities
    /// </summary>
    public interface IMessageReceiver :
        IDisposable
    {
        /// <summary>
        /// Configure all registered consumers, sagas, and activities on the receiver and handle the message
        /// </summary>
        /// <param name="queueName">The input entity name, used for the receiver InputAddress</param>
        /// <param name="message">The Service Bus message</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task Handle(string queueName, ServiceBusReceivedMessage message, CancellationToken cancellationToken);

        /// <summary>
        /// Configure all registered consumers, sagas, and activities on the receiver and handle the message
        /// </summary>
        /// <param name="topicPath">The input entity name, used for the receiver InputAddress</param>
        /// <param name="subscriptionName">The subscription name, should match the trigger</param>
        /// <param name="message">The Service Bus message</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task Handle(string topicPath, string subscriptionName, ServiceBusReceivedMessage message, CancellationToken cancellationToken);

        /// <summary>
        /// Configure the specified consumer on the receiver and handle the message
        /// </summary>
        /// <param name="queueName">The input entity name, used for the receiver InputAddress</param>
        /// <param name="message">The Service Bus message</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task HandleConsumer<TConsumer>(string queueName, ServiceBusReceivedMessage message, CancellationToken cancellationToken)
            where TConsumer : class, IConsumer;

        /// <summary>
        /// Configure the specified consumer on the receiver and handle the message
        /// </summary>
        /// <param name="topicPath">The input entity name, used for the receiver InputAddress</param>
        /// <param name="subscriptionName">The subscription name, should match the trigger</param>
        /// <param name="message">The Service Bus message</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task HandleConsumer<TConsumer>(string topicPath, string subscriptionName, ServiceBusReceivedMessage message, CancellationToken cancellationToken)
            where TConsumer : class, IConsumer;

        /// <summary>
        /// Configure the specified saga on the receiver and handle the message
        /// </summary>
        /// <param name="queueName">The input entity name, used for the receiver InputAddress</param>
        /// <param name="message">The Service Bus message</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task HandleSaga<TSaga>(string queueName, ServiceBusReceivedMessage message, CancellationToken cancellationToken)
            where TSaga : class, ISaga;

        /// <summary>
        /// Configure the specified saga on the receiver and handle the message
        /// </summary>
        /// <param name="topicPath">The input entity name, used for the receiver InputAddress</param>
        /// <param name="subscriptionName">The subscription name, should match the trigger</param>
        /// <param name="message">The Service Bus message</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task HandleSaga<TSaga>(string topicPath, string subscriptionName, ServiceBusReceivedMessage message, CancellationToken cancellationToken)
            where TSaga : class, ISaga;

        /// <summary>
        /// Configure the specified execute activity on the receiver and handle the message
        /// </summary>
        /// <param name="queueName">The input entity name, used for the receiver InputAddress</param>
        /// <param name="message">The Service Bus message</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task HandleExecuteActivity<TActivity>(string queueName, ServiceBusReceivedMessage message, CancellationToken cancellationToken)
            where TActivity : class;
    }
}
