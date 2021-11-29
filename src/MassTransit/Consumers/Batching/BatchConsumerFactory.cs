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
        readonly int _messageLimit;
        readonly TimeSpan _timeLimit;

        public BatchConsumerFactory(int messageLimit, TimeSpan timeLimit, IBatchCollector<TMessage> collector)
        {
            _messageLimit = messageLimit;
            _timeLimit = timeLimit;

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

            scope.Add("timeLimit", _timeLimit);
            scope.Add("messageLimit", _messageLimit);

            _collector.Probe(scope);
        }
    }
}
