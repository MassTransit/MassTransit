namespace MassTransit.Pipeline.ConsumerFactories
{
    using System;
    using System.Threading.Tasks;
    using Context;
    using GreenPipes;
    using Metadata;
    using Util;


    public class BatchConsumerFactory<TConsumer, TMessage> :
        IConsumerFactory<IConsumer<TMessage>>
        where TMessage : class
        where TConsumer : class, IConsumer<Batch<TMessage>>
    {
        readonly IConsumerFactory<TConsumer> _consumerFactory;
        readonly IPipe<ConsumerConsumeContext<TConsumer, Batch<TMessage>>> _consumerPipe;
        readonly int _messageLimit;
        readonly TimeSpan _timeLimit;
        readonly TaskScheduler _scheduler;
        BatchConsumer<TConsumer, TMessage> _currentConsumer;

        public BatchConsumerFactory(IConsumerFactory<TConsumer> consumerFactory, int messageLimit, TimeSpan timeLimit,
            IPipe<ConsumerConsumeContext<TConsumer, Batch<TMessage>>> consumerPipe)
        {
            _consumerFactory = consumerFactory;
            _messageLimit = messageLimit;
            _timeLimit = timeLimit;
            _consumerPipe = consumerPipe;

            _scheduler = new LimitedConcurrencyLevelTaskScheduler(1);
        }

        async Task IConsumerFactory<IConsumer<TMessage>>.Send<T>(ConsumeContext<T> context, IPipe<ConsumerConsumeContext<IConsumer<TMessage>, T>> next)
        {
            var messageContext = context as ConsumeContext<TMessage>;
            if (messageContext == null)
                throw new MessageException(typeof(T), $"Expected batch message type: {TypeMetadataCache<TMessage>.ShortName}");

            var consumer = await Task.Factory.StartNew(() => Add(messageContext), context.CancellationToken, TaskCreationOptions.None, _scheduler)
                .ConfigureAwait(false);

            await next.Send(new ConsumerConsumeContextScope<BatchConsumer<TConsumer, TMessage>, T>(context, consumer)).ConfigureAwait(false);
        }

        BatchConsumer<TConsumer, TMessage> Add(ConsumeContext<TMessage> context)
        {
            if (_currentConsumer != null)
            {
                if (context.GetRetryAttempt() > 0)
                    _currentConsumer.ForceComplete();
            }

            if (_currentConsumer == null || _currentConsumer.IsCompleted)
                _currentConsumer = new BatchConsumer<TConsumer, TMessage>(_messageLimit, _timeLimit, _scheduler, _consumerFactory, _consumerPipe);

            _currentConsumer.Add(context);

            BatchConsumer<TConsumer, TMessage> consumer = _currentConsumer;

            return consumer;
        }

        void IProbeSite.Probe(ProbeContext context)
        {
            var scope = context.CreateConsumerFactoryScope<IConsumer<TMessage>>("batch");

            scope.Add("timeLimit", _timeLimit);
            scope.Add("messageLimit", _messageLimit);

            _consumerFactory.Probe(scope);
            _consumerPipe.Probe(scope);
        }
    }
}
