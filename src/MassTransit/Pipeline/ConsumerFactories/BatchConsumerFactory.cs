namespace MassTransit.Pipeline.ConsumerFactories
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Context;
    using GreenPipes;
    using Metadata;
    using Util;


    public class BatchConsumerFactory<TConsumer, TMessage> :
        IConsumerFactory<IConsumer<TMessage>>,
        IAsyncDisposable
        where TMessage : class
        where TConsumer : class, IConsumer<Batch<TMessage>>
    {
        readonly ChannelExecutor _collector;
        readonly IConsumerFactory<TConsumer> _consumerFactory;
        readonly IPipe<ConsumerConsumeContext<TConsumer, Batch<TMessage>>> _consumerPipe;
        readonly ChannelExecutor _dispatcher;
        readonly int _messageLimit;
        readonly TimeSpan _timeLimit;
        readonly Func<ConsumeContext, object> _groupingExpression;
        readonly CurrentConsumersDictionary _currentConsumers = new CurrentConsumersDictionary();

        public BatchConsumerFactory(IConsumerFactory<TConsumer> consumerFactory, int messageLimit, int concurrencyLimit, TimeSpan timeLimit,
            Func<ConsumeContext, object> groupingExpression, IPipe<ConsumerConsumeContext<TConsumer, Batch<TMessage>>> consumerPipe)
        {
            _consumerFactory = consumerFactory;
            _messageLimit = messageLimit;
            _timeLimit = timeLimit;
            _groupingExpression = groupingExpression;
            _consumerPipe = CreateConsumePipe(consumerPipe);

            _collector = new ChannelExecutor(1);
            _dispatcher = new ChannelExecutor(concurrencyLimit);
        }

        public async ValueTask DisposeAsync()
        {
            await _collector.DisposeAsync().ConfigureAwait(false);

            await _dispatcher.DisposeAsync().ConfigureAwait(false);
        }

        async Task IConsumerFactory<IConsumer<TMessage>>.Send<T>(ConsumeContext<T> context, IPipe<ConsumerConsumeContext<IConsumer<TMessage>, T>> next)
        {
            var messageContext = context as ConsumeContext<TMessage>;
            if (messageContext == null)
                throw new MessageException(typeof(T), $"Expected batch message type: {TypeMetadataCache<TMessage>.ShortName}");

            BatchConsumer<TConsumer, TMessage> consumer = await _collector.Run(() => Add(messageContext), context.CancellationToken).ConfigureAwait(false);

            await next.Send(new ConsumerConsumeContextScope<BatchConsumer<TConsumer, TMessage>, T>(context, consumer)).ConfigureAwait(false);
        }

        IPipe<ConsumerConsumeContext<TConsumer, Batch<TMessage>>> CreateConsumePipe(IPipe<ConsumerConsumeContext<TConsumer, Batch<TMessage>>> consumerPipe)
        {
            return Pipe.ExecuteAsync<ConsumerConsumeContext<TConsumer, Batch<TMessage>>>(async ctx =>
            {
                try
                {
                    await consumerPipe.Send(ctx).ConfigureAwait(false);
                }
                finally
                {
                    var groupKey = GetGroupKey(ctx);
                    _currentConsumers.Release(groupKey);
                }
            });
        }

        void IProbeSite.Probe(ProbeContext context)
        {
            var scope = context.CreateConsumerFactoryScope<IConsumer<TMessage>>("batch");

            scope.Add("timeLimit", _timeLimit);
            scope.Add("messageLimit", _messageLimit);

            _consumerFactory.Probe(scope);
            _consumerPipe.Probe(scope);
        }

        async Task<BatchConsumer<TConsumer, TMessage>> Add(ConsumeContext<TMessage> context)
        {
            var groupKey = GetGroupKey(context);
            BatchConsumer<TConsumer, TMessage> consumer = _currentConsumers.Get(groupKey);

            if (consumer != null)
            {
                if (context.GetRetryAttempt() > 0)
                    await consumer.ForceComplete().ConfigureAwait(false);
            }

            if (consumer == null || consumer.IsCompleted)
            {
                consumer = new BatchConsumer<TConsumer, TMessage>(_messageLimit, _timeLimit, _collector, _dispatcher, _consumerFactory, _consumerPipe);
                _currentConsumers.Set(groupKey, consumer);
            }

            await consumer.Add(context).ConfigureAwait(false);

            return consumer;
        }

        object GetGroupKey(ConsumeContext context)
        {
            return _groupingExpression?.Invoke(context);
        }


        class CurrentConsumersDictionary
        {
            BatchConsumer<TConsumer, TMessage> _defaultConsumer;
            readonly Dictionary<object, BatchConsumer<TConsumer, TMessage>> _consumers = new Dictionary<object, BatchConsumer<TConsumer, TMessage>>();

            public BatchConsumer<TConsumer, TMessage> Get(object groupKey)
            {
                if (groupKey == null)
                    return _defaultConsumer;

                _consumers.TryGetValue(groupKey, out BatchConsumer<TConsumer, TMessage> consumer);
                return consumer;
            }

            public void Set(object groupKey, BatchConsumer<TConsumer, TMessage> consumer)
            {
                if (groupKey == null)
                {
                    _defaultConsumer = consumer;
                    return;
                }

                _consumers[groupKey] = consumer;
            }

            public void Release(object groupKey)
            {
                if (groupKey == null)
                {
                    _defaultConsumer = null;
                    return;
                }

                _consumers.Remove(groupKey);
            }
        }
    }
}
