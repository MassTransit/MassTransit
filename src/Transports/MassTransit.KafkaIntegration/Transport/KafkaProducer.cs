namespace MassTransit.KafkaIntegration.Transport
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Confluent.Kafka;
    using Context;
    using Contexts;
    using GreenPipes;
    using GreenPipes.Internals.Extensions;
    using Initializers;
    using Logging;
    using Transports;


    public class KafkaProducer<TKey, TValue> :
        IKafkaProducer<TKey, TValue>
        where TValue : class
    {
        readonly ConsumeContext _consumeContext;
        readonly IKafkaProducerContext<TKey, TValue> _producerContext;
        readonly KafkaTopicAddress _topicAddress;

        public KafkaProducer(KafkaTopicAddress topicAddress, IKafkaProducerContext<TKey, TValue> producerContext, ConsumeContext consumeContext = null)
        {
            _topicAddress = topicAddress;
            _producerContext = producerContext;
            _consumeContext = consumeContext;
        }

        public Task Produce(TKey key, TValue value, CancellationToken cancellationToken = default)
        {
            return Produce(key, value, Pipe.Empty<KafkaProduceContext<TValue>>(), cancellationToken);
        }

        public async Task Produce(TKey key, TValue value, IPipe<KafkaProduceContext<TValue>> pipe, CancellationToken cancellationToken)
        {
            LogContext.SetCurrentIfNull(_producerContext.LogContext);

            var context = new MessageKafkaProduceContext<TValue>(value, cancellationToken);

            if (_consumeContext != null)
                context.TransferConsumeContextHeaders(_consumeContext);

            context.DestinationAddress = _topicAddress;

            if (pipe.IsNotEmpty())
                await pipe.Send(context).ConfigureAwait(false);

            context.SourceAddress ??= _producerContext.HostAddress;
            context.ConversationId ??= NewId.NextGuid();

            StartedActivity? activity = LogContext.IfEnabled(OperationName.Transport.Send)?.StartSendActivity(context,
                (nameof(context.Partition), context.Partition.ToString()));
            try
            {
                if (_producerContext.SendObservers.Count > 0)
                    await _producerContext.SendObservers.PreSend(context).ConfigureAwait(false);

                var message = new Message<TKey, TValue>
                {
                    Key = key,
                    Value = context.Message
                };

                if (context.SentTime.HasValue)
                    message.Timestamp = new Timestamp(context.SentTime.Value);

                message.Headers = _producerContext.HeadersSerializer.Serialize(context);

                var topic = new TopicPartition(_topicAddress.Topic, _topicAddress.Partition ?? context.Partition);

                await _producerContext.Produce(topic, message, context.CancellationToken).ConfigureAwait(false);

                context.LogSent();

                if (_producerContext.SendObservers.Count > 0)
                    await _producerContext.SendObservers.PostSend(context).ConfigureAwait(false);
            }
            catch (Exception exception)
            {
                context.LogFaulted(exception);

                if (_producerContext.SendObservers.Count > 0)
                    await _producerContext.SendObservers.SendFault(context, exception).ConfigureAwait(false);

                throw;
            }
            finally
            {
                activity?.Stop();
            }
        }

        public Task Produce(TKey key, object values, CancellationToken cancellationToken = default)
        {
            return Produce(key, values, Pipe.Empty<KafkaProduceContext<TValue>>(), cancellationToken);
        }

        public Task Produce(TKey key, object values, IPipe<KafkaProduceContext<TValue>> pipe, CancellationToken cancellationToken = default)
        {
            Task<InitializeContext<TValue>> messageTask = MessageInitializerCache<TValue>.Initialize(values, cancellationToken);
            if (messageTask.IsCompletedSuccessfully())
                return Produce(key, messageTask.GetAwaiter().GetResult().Message, pipe, cancellationToken);

            async Task ProduceAsync()
            {
                InitializeContext<TValue> context = await messageTask.ConfigureAwait(false);

                await Produce(key, context.Message, pipe, cancellationToken).ConfigureAwait(false);
            }

            return ProduceAsync();
        }

        public ConnectHandle ConnectSendObserver(ISendObserver observer)
        {
            return _producerContext.SendObservers.Connect(observer);
        }
    }
}
