namespace MassTransit
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Azure.Messaging.EventHubs;


    /// <summary>
    /// Called by an Azure Function to handle messages using configured consumers, sagas, or activities
    /// </summary>
    public interface IEventReceiver :
        IDisposable
    {
        /// <summary>
        /// Configure all registered consumers, sagas, and activities on the receiver and handle the message
        /// </summary>
        /// <param name="entityName">The input entity name, used for the receiver InputAddress</param>
        /// <param name="message">The Service Bus message</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task Handle(string entityName, EventData message, CancellationToken cancellationToken);

        /// <summary>
        /// Configure the specified consumer on the receiver and handle the message
        /// </summary>
        /// <param name="entityName">The input entity name, used for the receiver InputAddress</param>
        /// <param name="message">The Service Bus message</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task HandleConsumer<TConsumer>(string entityName, EventData message, CancellationToken cancellationToken)
            where TConsumer : class, IConsumer;

        /// <summary>
        /// Configure the specified saga on the receiver and handle the message
        /// </summary>
        /// <param name="entityName">The input entity name, used for the receiver InputAddress</param>
        /// <param name="message">The Service Bus message</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task HandleSaga<TSaga>(string entityName, EventData message, CancellationToken cancellationToken)
            where TSaga : class, ISaga;

        /// <summary>
        /// Configure the specified execute activity on the receiver and handle the message
        /// </summary>
        /// <param name="entityName">The input entity name, used for the receiver InputAddress</param>
        /// <param name="message">The Service Bus message</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task HandleExecuteActivity<TActivity>(string entityName, EventData message, CancellationToken cancellationToken)
            where TActivity : class;
    }
}
