namespace MassTransit.KafkaIntegration
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Confluent.Kafka;
    using Logging;
    using MassTransit.Configuration;
    using MassTransit.Middleware;


    public class KafkaProducerContext :
        BasePipeContext,
        ProducerContext
    {
        readonly CancellationTokenSource _cancellationTokenSource;
        readonly IHostConfiguration _hostConfiguration;
        readonly IProducer<byte[], byte[]> _producer;

        public KafkaProducerContext(ProducerBuilder<byte[], byte[]> producerBuilder, IHostConfiguration hostConfiguration, CancellationToken cancellationToken)
            : base(cancellationToken)
        {
            _hostConfiguration = hostConfiguration;
            _producer = producerBuilder
                .SetErrorHandler(OnError)
                .SetLogHandler((_, message) => hostConfiguration.SendLogContext?.Debug?.Log(message.Message))
                .Build();
            _cancellationTokenSource = new CancellationTokenSource();
        }

        public Task Produce(TopicPartition partition, Message<byte[], byte[]> message, CancellationToken cancellationToken)
        {
            return _producer.ProduceAsync(partition, message, cancellationToken);
        }

        public void Dispose()
        {
            LogContext.SetCurrentIfNull(_hostConfiguration.SendLogContext);

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
                LogContext.Error?.Log(e, "Failed to dispose producer: {producerName}", _producer.Name);
            }
            finally
            {
                timeoutTokenSource.Dispose();
                _cancellationTokenSource.Dispose();
                _producer.Dispose();
            }
        }

        void OnError(IProducer<byte[], byte[]> producer, Error error)
        {
            LogContext.SetCurrentIfNull(_hostConfiguration.SendLogContext);

            EnabledLogger? logger = error.IsFatal ? LogContext.Error : LogContext.Warning;
            logger?.Log("Error ({code}): {reason} on producer: {producerName}", error.Code, error.Reason, producer.Name);

            if (!_cancellationTokenSource.IsCancellationRequested && error.IsLocalError)
                _cancellationTokenSource.Cancel();
        }
    }
}
