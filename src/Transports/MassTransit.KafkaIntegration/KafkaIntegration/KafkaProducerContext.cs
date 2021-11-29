namespace MassTransit.KafkaIntegration
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Confluent.Kafka;
    using Logging;
    using MassTransit.Middleware;
    using Serializers;


    public class KafkaProducerContext<TKey, TValue> :
        BasePipeContext,
        ProducerContext<TKey, TValue>
        where TValue : class
    {
        readonly CancellationTokenSource _cancellationTokenSource;
        readonly ILogContext _logContext;
        readonly IProducer<TKey, TValue> _producer;

        public KafkaProducerContext(ProducerBuilder<TKey, TValue> producerBuilder, IHeadersSerializer headersSerializer, ILogContext logContext,
            CancellationToken cancellationToken)
            : base(cancellationToken)
        {
            _logContext = logContext;
            _producer = producerBuilder
                .SetErrorHandler(OnError)
                .Build();
            _cancellationTokenSource = new CancellationTokenSource();
            HeadersSerializer = headersSerializer;
        }

        public IHeadersSerializer HeadersSerializer { get; }

        public Task Produce(TopicPartition partition, Message<TKey, TValue> message, CancellationToken cancellationToken)
        {
            return _producer.ProduceAsync(partition, message, cancellationToken);
        }

        public void Dispose()
        {
            var timeoutTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(30));
            var cts = CancellationTokenSource.CreateLinkedTokenSource(timeoutTokenSource.Token, _cancellationTokenSource.Token);
            try
            {
                _producer.Flush(cts.Token);
            }
            catch (OperationCanceledException)
            {
            }
            catch (Exception e)
            {
                _logContext.Error?.Log(e, "Failed to dispose producer: {producerName}", _producer.Name);
            }
            finally
            {
                timeoutTokenSource.Dispose();
                _cancellationTokenSource.Dispose();
                _producer.Dispose();
            }
        }

        void OnError(IProducer<TKey, TValue> producer, Error error)
        {
            EnabledLogger? logger = error.IsFatal ? _logContext.Error : _logContext.Warning;
            logger?.Log("Error ({code}): {reason} on producer: {producerName}", error.Code, error.Reason, producer.Name);

            if (!_cancellationTokenSource.IsCancellationRequested && error.IsLocalError)
                _cancellationTokenSource.Cancel();
        }
    }
}
