#nullable enable
namespace MassTransit.EntityFrameworkCoreIntegration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Context;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Storage;
    using Middleware;
    using Middleware.Outbox;


    public class DbContextOutboxConsumeContext<TDbContext, TMessage> :
        OutboxConsumeContextProxy<TMessage>,
        DbTransactionContext
        where TDbContext : DbContext
        where TMessage : class
    {
        readonly TDbContext _dbContext;
        readonly InboxState _inboxState;
        readonly IDbContextTransaction _transaction;

        public DbContextOutboxConsumeContext(ConsumeContext<TMessage> context, OutboxConsumeOptions options, TDbContext dbContext,
            IDbContextTransaction transaction, InboxState inboxState)
            : base(context, options)
        {
            _dbContext = dbContext;
            _transaction = transaction;
            _inboxState = inboxState;
        }

        public override Guid? MessageId => _inboxState.MessageId;

        public override bool ContinueProcessing { get; set; } = true;

        public override bool IsMessageConsumed => _inboxState.Consumed.HasValue;
        public override bool IsOutboxDelivered => _inboxState.Delivered.HasValue;
        public override int ReceiveCount => _inboxState.ReceiveCount;
        public override long? LastSequenceNumber => _inboxState.LastSequenceNumber;

        public Guid TransactionId => _transaction.TransactionId;

        public override async Task SetConsumed()
        {
            _inboxState.Consumed = DateTime.UtcNow;
            _dbContext.Update(_inboxState);

            await _dbContext.SaveChangesAsync(CancellationToken);

            LogContext.Debug?.Log("Outbox Consumed: {MessageId} {Consumed}", MessageId, _inboxState.Consumed);
        }

        public override async Task SetDelivered()
        {
            _inboxState.Delivered = DateTime.UtcNow;
            _dbContext.Update(_inboxState);

            await _dbContext.SaveChangesAsync(CancellationToken);

            LogContext.Debug?.Log("Outbox Delivered: {MessageId} {Delivered}", MessageId, _inboxState.Delivered);
        }

        public override async Task<List<OutboxMessageContext>> LoadOutboxMessages()
        {
            List<OutboxMessage> messages = await _dbContext.Set<OutboxMessage>()
                .Where(x => x.InboxMessageId == MessageId && x.InboxConsumerId == ConsumerId)
                .OrderBy(x => x.SequenceNumber)
                .AsNoTracking()
                .ToListAsync(CancellationToken);

            for (var i = 0; i < messages.Count; i++)
                messages[i].Deserialize(SerializerContext);

            return messages.Cast<OutboxMessageContext>().ToList();
        }

        public override Task NotifyOutboxMessageDelivered(OutboxMessageContext message)
        {
            _inboxState.LastSequenceNumber = message.SequenceNumber;
            _dbContext.Update(_inboxState);

            return _dbContext.SaveChangesAsync(CancellationToken);
        }

        public override async Task RemoveOutboxMessages()
        {
            List<OutboxMessage> messages = await _dbContext.Set<OutboxMessage>()
                .Where(x => x.InboxMessageId == MessageId && x.InboxConsumerId == ConsumerId)
                .OrderBy(x => x.SequenceNumber)
                .ToListAsync(CancellationToken);

            _dbContext.RemoveRange(messages);

            await _dbContext.SaveChangesAsync(CancellationToken);

            if (messages.Count > 0)
                LogContext.Debug?.Log("Outbox removed {Count} messages: {MessageId}", messages.Count, MessageId);
        }

        public override async Task AddSend<T>(SendContext<T> context)
            where T : class
        {
            if (context.MessageId.HasValue == false)
                throw new MessageException(typeof(T), "The SendContext MessageId must be present");

            var body = context.Serializer.GetMessageBody(context);

            var now = DateTime.UtcNow;

            var outboxMessage = new OutboxMessage
            {
                MessageId = context.MessageId.Value,
                ConversationId = context.ConversationId,
                CorrelationId = context.CorrelationId,
                InitiatorId = context.InitiatorId,
                RequestId = context.RequestId,
                SourceAddress = context.SourceAddress,
                DestinationAddress = context.DestinationAddress,
                ResponseAddress = context.ResponseAddress,
                FaultAddress = context.FaultAddress,
                SentTime = context.SentTime ?? now,
                ContentType = context.ContentType?.ToString() ?? context.Serialization.DefaultContentType.ToString(),
                Body = body.GetString(),
                InboxMessageId = MessageId,
                InboxConsumerId = ConsumerId
            };

            if (context.TimeToLive.HasValue)
                outboxMessage.ExpirationTime = now + context.TimeToLive;

            if (context.Delay.HasValue)
                outboxMessage.EnqueueTime = now + context.Delay;

            var headers = SerializerContext.SerializeDictionary(context.Headers.GetAll());
            if (headers.Length > 0)
                outboxMessage.Headers = headers.GetString();

            if (context is TransportSendContext<T> transportSendContext)
            {
                var properties = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);

                transportSendContext.WritePropertiesTo(properties);
                if (properties.Count > 0)
                    outboxMessage.Properties = SerializerContext.SerializeDictionary(properties).GetString();
            }

            await _dbContext.AddAsync(outboxMessage, CancellationToken).ConfigureAwait(false);
        }
    }
}
