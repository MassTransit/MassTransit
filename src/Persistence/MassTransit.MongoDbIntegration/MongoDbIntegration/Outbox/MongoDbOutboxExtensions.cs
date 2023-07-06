namespace MassTransit.MongoDbIntegration.Outbox
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Context;


    public static class MongoDbOutboxExtensions
    {
        static long _nextSequenceNumber;

        public static Task AddSend<T>(this MongoDbCollectionContext<OutboxMessage> collection, SendContext<T> context, IObjectDeserializer deserializer,
            Guid? inboxMessageId = null, Guid? inboxConsumerId = null, Guid? outboxId = null)
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
                MessageType = string.Join(";", context.SupportedMessageTypes),
                Body = body.GetString(),
                InboxMessageId = inboxMessageId,
                InboxConsumerId = inboxConsumerId,
                OutboxId = outboxId,
                SequenceNumber = Interlocked.Increment(ref _nextSequenceNumber)
            };

            if (context.TimeToLive.HasValue)
                outboxMessage.ExpirationTime = now + context.TimeToLive;

            if (context.Delay.HasValue)
                outboxMessage.EnqueueTime = now + context.Delay;

            outboxMessage.Headers = deserializer.SerializeDictionary(context.Headers.GetAll());

            if (context is TransportSendContext<T> transportSendContext)
            {
                var properties = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);

                transportSendContext.WritePropertiesTo(properties);
                outboxMessage.Properties = deserializer.SerializeDictionary(properties);
            }

            return collection.InsertOne(outboxMessage, context.CancellationToken);
        }
    }
}
