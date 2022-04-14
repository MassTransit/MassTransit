namespace MassTransit.Middleware.Outbox
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;


    public class InMemoryOutboxMessageRepository
    {
        readonly Dictionary<InMemoryInboxMessageKey, InMemoryInboxMessage> _dictionary;

        readonly SemaphoreSlim _inUse = new SemaphoreSlim(1);

        public InMemoryOutboxMessageRepository()
        {
            _dictionary = new Dictionary<InMemoryInboxMessageKey, InMemoryInboxMessage>(InMemoryInboxMessageKey.Comparer);
        }

        public Task MarkInUse(CancellationToken cancellationToken)
        {
            return _inUse.WaitAsync(cancellationToken);
        }

        public async Task<InMemoryInboxMessage> Lock(Guid messageId, Guid consumerId, CancellationToken cancellationToken)
        {
            var key = new InMemoryInboxMessageKey(messageId, consumerId);

            if (!_dictionary.TryGetValue(key, out var existing))
            {
                existing = new InMemoryInboxMessage(messageId, consumerId)
                {
                    Received = DateTime.UtcNow,
                    ReceiveCount = 0
                };

                _dictionary.Add(key, existing);
            }

            await existing.MarkInUse(cancellationToken).ConfigureAwait(false);

            return existing;
        }

        public void Release()
        {
            _inUse.Release();
        }
    }
}
