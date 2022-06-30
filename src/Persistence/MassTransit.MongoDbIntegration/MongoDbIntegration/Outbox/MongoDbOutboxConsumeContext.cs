namespace MassTransit.MongoDbIntegration.Outbox
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Middleware;
    using Middleware.Outbox;
    using MongoDB.Driver;


    public class MongoDbOutboxConsumeContext<TMessage> :
        OutboxConsumeContextProxy<TMessage>
        where TMessage : class
    {
        readonly InboxState _inboxState;
        readonly MongoDbCollectionContext<OutboxMessage> _collection;

        public MongoDbOutboxConsumeContext(ConsumeContext<TMessage> context, OutboxConsumeOptions options, InboxState inboxState, MongoDbContext dbContext)
            : base(context, options)
        {
            _inboxState = inboxState;
            _collection = dbContext.GetCollection<OutboxMessage>();
        }

        public override Guid? MessageId => _inboxState.MessageId;

        public override bool ContinueProcessing { get; set; } = true;

        public override bool IsMessageConsumed => _inboxState.Consumed.HasValue;
        public override bool IsOutboxDelivered => _inboxState.Delivered.HasValue;
        public override int ReceiveCount => _inboxState.ReceiveCount;
        public override long? LastSequenceNumber => _inboxState.LastSequenceNumber;

        public override async Task SetConsumed()
        {
            _inboxState.Consumed = DateTime.UtcNow;

            LogContext.Debug?.Log("Outbox Consumed: {MessageId} {Consumed}", MessageId, _inboxState.Consumed);
        }

        public override async Task SetDelivered()
        {
            _inboxState.Delivered = DateTime.UtcNow;

            LogContext.Debug?.Log("Outbox Delivered: {MessageId} {Delivered}", MessageId, _inboxState.Delivered);
        }

        public override async Task<List<OutboxMessageContext>> LoadOutboxMessages()
        {
            var lastSequenceNumber = LastSequenceNumber ?? 0;

            FilterDefinitionBuilder<OutboxMessage> builder = Builders<OutboxMessage>.Filter;
            FilterDefinition<OutboxMessage> filter = builder.Eq(x => x.InboxMessageId, MessageId) & builder.Eq(x => x.InboxConsumerId, ConsumerId)
                & builder.Gt(x => x.SequenceNumber, lastSequenceNumber);

            List<OutboxMessage> messages = await _collection.Find(filter)
                .Sort(Builders<OutboxMessage>.Sort.Ascending(x => x.SequenceNumber))
                .Limit(Options.MessageDeliveryLimit + 1)
                .ToListAsync(CancellationToken).ConfigureAwait(false);

            for (var i = 0; i < messages.Count; i++)
                messages[i].Deserialize(SerializerContext);

            return messages.Cast<OutboxMessageContext>().ToList();
        }

        public override Task NotifyOutboxMessageDelivered(OutboxMessageContext message)
        {
            _inboxState.LastSequenceNumber = message.SequenceNumber;

            return Task.CompletedTask;
        }

        public override async Task RemoveOutboxMessages()
        {
            FilterDefinitionBuilder<OutboxMessage> builder = Builders<OutboxMessage>.Filter;
            FilterDefinition<OutboxMessage> filter = builder.Eq(x => x.InboxMessageId, MessageId) & builder.Eq(x => x.InboxConsumerId, ConsumerId);

            var messages = await _collection.DeleteMany(filter, CancellationToken).ConfigureAwait(false);

            if (messages.DeletedCount > 0)
                LogContext.Debug?.Log("Outbox removed {Count} messages: {MessageId}", messages.DeletedCount, MessageId);
        }

        public override Task AddSend<T>(SendContext<T> context)
            where T : class
        {
            return _collection.AddSend(context, SerializerContext, MessageId, ConsumerId);
        }
    }
}
