namespace MassTransit.Tests.Audit
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using MassTransit.Audit;
    using MassTransit.Caching;
    using Metadata;


    public class InMemoryAuditStore :
        IMessageAuditStore,
        IEnumerable<Task<InMemoryAuditStore.AuditRecord>>
    {
        readonly ICache<AuditRecord> _audits;
        readonly IIndex<Guid, AuditRecord> _messageId;

        public InMemoryAuditStore()
        {
            var cacheSettings = new CacheSettings(10000, TimeSpan.FromMinutes(1), TimeSpan.FromMinutes(60));

            _audits = new GreenCache<AuditRecord>(cacheSettings);
            _messageId = _audits.AddIndex("messageId", x => x.Metadata.MessageId.Value);
        }

        public IEnumerator<Task<AuditRecord>> GetEnumerator()
        {
            return _audits.GetAll().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        Task IMessageAuditStore.StoreMessage<T>(T message, MessageAuditMetadata metadata)
        {
            _audits.Add(new AuditRecord<T>(message, metadata));

            return Task.CompletedTask;
        }


        public interface AuditRecord
        {
            string MessageType { get; }
            MessageAuditMetadata Metadata { get; }
        }


        public class AuditRecord<T> :
            AuditRecord
            where T : class
        {
            public AuditRecord(T message, MessageAuditMetadata metadata)
            {
                Message = message;
                MessageType = TypeCache<T>.ShortName;
                Metadata = metadata;
            }

            public T Message { get; }
            public string MessageType { get; }
            public MessageAuditMetadata Metadata { get; }
        }
    }
}
