namespace MassTransit.KafkaIntegration
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Confluent.Kafka;
    using Context;
    using Util;


    public interface IKafkaSubscription
    {
        Task Subscribe(CancellationToken cancellationToken);
        Task Unsubscribe(CancellationToken cancellationToken);
    }


    public class KafkaSubscription<TKey, TValue> :
        IKafkaSubscription
        where TValue : class
    {
        readonly IConsumer<TKey, TValue> _consumer;
        readonly bool _isAutoCommitEnabled;
        readonly ILogContext _logContext;
        readonly string _topic;
        CancellationTokenSource _cancellationTokenSource;
        Task _consumerTask;

        public KafkaSubscription(string topic, IConsumer<TKey, TValue> consumer, ILogContext logContext, bool isAutoCommitEnabled)
        {
            _topic = topic;
            _consumer = consumer;
            _logContext = logContext;
            _isAutoCommitEnabled = isAutoCommitEnabled;
        }

        public Task Subscribe(CancellationToken cancellationToken)
        {
            _cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

            _consumerTask = Task.Run(async () =>
            {
                await Task.Yield();
                while (!_cancellationTokenSource.IsCancellationRequested)
                {
                    try
                    {
                        ConsumeResult<TKey, TValue> message = _consumer.Consume(cancellationToken);

                        if (!_isAutoCommitEnabled)
                            _consumer.Commit(message);
                    }
                    catch (Exception e)
                    {
                        _logContext.Error?.Log(e, "Kafka subscription: {topicName} exception", _topic);
                        throw;
                    }
                }
            }, cancellationToken);

            _logContext.Info?.Log("Kafka subscription: {topicName} starting", _topic);
            _consumer.Subscribe(_topic);

            return _consumerTask.IsCompleted ? _consumerTask : TaskUtil.Completed;
        }

        public async Task Unsubscribe(CancellationToken cancellationToken)
        {
            _logContext.Info?.Log("Kafka subscription: {topicName} stopping", _topic);
            _consumer.Unsubscribe();

            await _consumerTask.ConfigureAwait(false);
            _cancellationTokenSource.Cancel();

            _consumer.Dispose();
            _cancellationTokenSource.Dispose();
        }
    }
}
