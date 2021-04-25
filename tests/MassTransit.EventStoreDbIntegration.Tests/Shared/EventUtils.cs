using System;
using System.IO;
using System.Threading.Tasks;
using EventStore.Client;
using MassTransit.Context;
using MassTransit.EventStoreDbIntegration.Serializers;

namespace MassTransit.EventStoreDbIntegration.Tests
{
    public static class EventUtils
    {
        public static async Task<EventData> PrepareMessage<TEvent>(TEvent @event, IMessageSerializer serializer,
            Guid conversationId, Guid initiatorId, Guid correlationId, Guid? messageId = null)
            where TEvent : class
        {
            var context = new MessageSendContext<TEvent>(@event);

            context.ConversationId = conversationId; //Equivalent to EventStoreDB $correlationId
            context.InitiatorId = initiatorId;       //Equivalent to EventStoreDB $causationId
            context.CorrelationId = correlationId;   //The instance id used for correlation in sagas
            context.MessageId = messageId ?? Guid.NewGuid();

            await using (var stream = new MemoryStream())
            {
                serializer.Serialize(stream, context);
                stream.Flush();

                var metadata = DictionaryHeadersSerde.Serializer.Serialize(context);

                var preparedMessage = new EventData(
                    Uuid.FromGuid(context.MessageId.Value),
                    @event.GetType().Name,
                    stream.ToArray(),
                    metadata);

                return preparedMessage;
            }
        }
    }
}
