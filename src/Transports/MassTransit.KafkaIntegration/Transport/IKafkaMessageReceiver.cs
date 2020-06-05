namespace MassTransit.KafkaIntegration.Transport
{
    using System.Threading;
    using System.Threading.Tasks;
    using Confluent.Kafka;
    using GreenPipes;
    using Pipeline;
    using Transports;
    using Transports.Metrics;


    public interface IKafkaMessageReceiver<TKey, TValue> :
        IDispatchMetrics,
        IReceiveObserverConnector,
        IReceiveTransportObserverConnector,
        IPublishObserverConnector,
        ISendObserverConnector,
        IConsumeMessageObserverConnector,
        IConsumeObserverConnector,
        IProbeSite
        where TValue : class
    {
        /// <summary>
        /// Handles the <paramref name="result" />
        /// </summary>
        /// <param name="result"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task Handle(ConsumeResult<TKey, TValue> result, CancellationToken cancellationToken);
    }
}
