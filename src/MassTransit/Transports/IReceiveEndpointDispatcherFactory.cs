namespace MassTransit.Transports
{
    using System;
    using Courier;
    using Saga;


    public interface IReceiveEndpointDispatcherFactory :
        IAsyncDisposable
    {
        /// <summary>
        /// Creates a single receiver with all configured consumers, sagas, etc.
        /// Note that if any other receivers are created for specific consumers or sagas, those consumers and sagas will
        /// not be included in this receiver as they've already been configured.
        /// </summary>
        /// <param name="queueName"></param>
        /// <returns></returns>
        IReceiveEndpointDispatcher CreateReceiver(string queueName);

        IReceiveEndpointDispatcher CreateConsumerReceiver<T>(string queueName)
            where T : class, IConsumer;

        IReceiveEndpointDispatcher CreateSagaReceiver<T>(string queueName)
            where T : class, ISaga;

        IReceiveEndpointDispatcher CreateExecuteActivityReceiver<T>(string queueName)
            where T : class, IExecuteActivity;
    }
}
