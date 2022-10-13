namespace MassTransit.Batching
{
    using System;
    using System.Threading.Tasks;
    using Context;


    public class BatchConsumerFactory<TMessage> :
        IConsumerFactory<BatchConsumer<TMessage>>,
        IAsyncDisposable
        where TMessage : class
    {
        readonly IBatchCollector<TMessage> _collector;
        readonly BatchOptions _options;

        public BatchConsumerFactory(BatchOptions options, IBatchCollector<TMessage>
            collector)
        {
            _options = options;
            _collector = collector;
        }

        public ValueTask DisposeAsync()
        {
            return _collector.DisposeAsync();
        }

        public virtual async Task Send<T>(ConsumeContext<T> context, IPipe<ConsumerConsumeContext<BatchConsumer<TMessage>, T>> next)
            where T : class
        {
            var messageContext = context as ConsumeContext<TMessage>;
            if (messageContext == null)
                throw new MessageException(typeof(T), $"Expected batch message type: {TypeCache<TMessage>.ShortName}");

            BatchConsumer<TMessage> consumer = await _collector.Collect(messageContext).ConfigureAwait(false);

            try
            {
                await next.Send(new ConsumerConsumeContextProxy<BatchConsumer<TMessage>, T>(context, consumer)).ConfigureAwait(false);
            }
            finally
            {
                if (consumer.IsCompleted)
                    await _collector.Complete(messageContext, consumer).ConfigureAwait(false);
            }
        }

        public void Probe(ProbeContext context)
        {
            var scope = context.CreateConsumerFactoryScope<IConsumer<TMessage>>("batch");

            scope.Add("timeLimit", _options.TimeLimit);
            scope.Add("timeLimitStart", _options.TimeLimitStart);
            scope.Add("messageLimit", _options.MessageLimit);
            scope.Add("concurrencyLimit", _options.ConcurrencyLimit);

            _collector.Probe(scope);
        }
    }
}
