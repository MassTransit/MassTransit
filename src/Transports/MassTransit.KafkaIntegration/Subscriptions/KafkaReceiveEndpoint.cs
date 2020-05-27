namespace MassTransit.KafkaIntegration.Subscriptions
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Confluent.Kafka;
    using Context;
    using Events;
    using Transport;
    using Transports;
    using Transports.Metrics;
    using Util;


    public class KafkaReceiveEndpoint<TKey, TValue> :
        ReceiveEndpoint,
        IKafkaReceiveEndpoint,
        DeliveryMetrics
        where TValue : class
    {
        readonly IConsumer<TKey, TValue> _consumer;
        readonly Uri _inputAddress;
        readonly bool _isAutoCommitEnabled;
        readonly ILogContext _logContext;
        readonly IKafkaReceiver<TKey, TValue> _receiver;
        readonly string _topic;
        CancellationTokenSource _cancellationTokenSource;
        Task _consumerTask;
        long _deliveredCount;

        public KafkaReceiveEndpoint(string topic, IConsumer<TKey, TValue> consumer, IKafkaReceiver<TKey, TValue> receiver, ReceiveEndpointContext context,
            ILogContext logContext, bool isAutoCommitEnabled)
            : base(receiver, context)
        {
            _topic = topic;
            _consumer = consumer;
            _receiver = receiver;
            _logContext = logContext;
            _isAutoCommitEnabled = isAutoCommitEnabled;
            _inputAddress = context.InputAddress;
            _deliveredCount = 0;
        }

        public long DeliveryCount => _deliveredCount;
        public int ConcurrentDeliveryCount => 0;

        public Task Connect(CancellationToken cancellationToken)
        {
            LogContext.SetCurrentIfNull(_logContext);

            _cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

            _consumerTask = Task.Run(async () =>
            {
                await _receiver.Ready(new ReceiveTransportReadyEvent(_inputAddress)).ConfigureAwait(false);

                while (!_cancellationTokenSource.IsCancellationRequested)
                {
                    LogContext.SetCurrentIfNull(_logContext);
                    try
                    {
                        ConsumeResult<TKey, TValue> message = _consumer.Consume(_cancellationTokenSource.Token);

                        await _receiver.Handle(message, cancellationToken).ConfigureAwait(false);

                        if (!_isAutoCommitEnabled)
                            _consumer.Commit(message);

                        Interlocked.Increment(ref _deliveredCount);
                    }
                    catch (OperationCanceledException e) when (e.CancellationToken == _cancellationTokenSource.Token)
                    {
                    }
                    catch (Exception e)
                    {
                        LogContext.Error?.Log(e, "Kafka subscription: {topicName} exception", _topic);
                        await _receiver.Faulted(new ReceiveTransportFaultedEvent(_inputAddress, e)).ConfigureAwait(false);
                        throw;
                    }
                }
            }, cancellationToken);

            LogContext.Info?.Log("Kafka subscription: {topicName} starting", _topic);
            _consumer.Subscribe(_topic);

            return _consumerTask.IsCompleted ? _consumerTask : TaskUtil.Completed;
        }

        public async Task Disconnect(CancellationToken cancellationToken)
        {
            LogContext.SetCurrentIfNull(_logContext);
            try
            {
                LogContext.Info?.Log("Kafka subscription: {topicName} stopping", _topic);

                _cancellationTokenSource.Cancel();
                await _consumerTask.ConfigureAwait(false);

                _consumer.Close();
                _consumer.Dispose();

                _cancellationTokenSource.Dispose();
            }
            catch (Exception e)
            {
                LogContext.Error?.Log(e, "Error occured while stopping kafka consumer: {topicName}", _topic);
            }
            finally
            {
                await _receiver.Completed(new ReceiveTransportCompletedEvent(_inputAddress, this)).ConfigureAwait(false);
            }
        }
    }
}
