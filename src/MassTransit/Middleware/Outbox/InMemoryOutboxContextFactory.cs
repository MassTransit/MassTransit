namespace MassTransit.Middleware.Outbox
{
    using System.Threading.Tasks;


    public class InMemoryOutboxContextFactory :
        IOutboxContextFactory<InMemoryOutboxMessageRepository>
    {
        readonly InMemoryOutboxMessageRepository _messageRepository;

        public InMemoryOutboxContextFactory(InMemoryOutboxMessageRepository messageRepository)
        {
            _messageRepository = messageRepository;
        }

        public async Task Send<T>(ConsumeContext<T> context, OutboxConsumeOptions options, IPipe<OutboxConsumeContext<T>> next)
            where T : class
        {
            var updateDeliveryCount = true;
            var continueProcessing = true;

            var messageId = context.GetOriginalMessageId() ?? throw new MessageException(typeof(T), "MessageId required to use the outbox");

            while (continueProcessing)
            {
                await _messageRepository.MarkInUse(context.CancellationToken).ConfigureAwait(false);

                InMemoryInboxMessage inboxMessage = null;
                try
                {
                    inboxMessage = await _messageRepository.Lock(messageId, options.ConsumerId, context.CancellationToken).ConfigureAwait(false);
                }
                finally
                {
                    _messageRepository.Release();
                }

                try
                {
                    if (updateDeliveryCount)
                        inboxMessage.ReceiveCount++;

                    updateDeliveryCount = false;

                    var outboxContext = new InMemoryOutboxConsumeContext<T>(context, options, inboxMessage);

                    await next.Send(outboxContext).ConfigureAwait(false);

                    continueProcessing = outboxContext.ContinueProcessing;
                }
                finally
                {
                    inboxMessage.Release();
                }
            }
        }

        public void Probe(ProbeContext context)
        {
            context.CreateFilterScope("inMemoryOutboxContextFactory");
        }
    }
}
