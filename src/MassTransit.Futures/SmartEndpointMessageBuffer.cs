namespace MassTransit
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using GreenPipes.Caching;
    using Metadata;
    using Util;


    public class SmartEndpointMessageBuffer :
        IMessageBuffer
    {
        readonly ICache<IMessageEvent> _events;
        readonly IIndex<Guid, IMessageEvent> _messageId;

        public SmartEndpointMessageBuffer()
        {
            var cacheSettings = new CacheSettings(1000, TimeSpan.FromMinutes(1), TimeSpan.FromHours(1));

            _events = new GreenCache<IMessageEvent>(cacheSettings);
            _messageId = _events.AddIndex("messageId", x => x.MessageId.Value);
        }

        Task IMessageBuffer.Add<T>(IMessageEvent<T> messageEvent)
        {
            _events.Add(messageEvent);

            return TaskUtil.Completed;
        }

    }


    public class MessageBundler
    {
        readonly ISendEndpoint _sendEndpoint;

        public MessageBundler(ISendEndpoint sendEndpoint)
        {
            _sendEndpoint = sendEndpoint;
        }

        public void Bundle(IEnumerable<IMessageEvent> messageEvents)
        {
            (Type Key, (string DestinationAddress, IMessageEvent[] events)[] Destinations)[] buckets = messageEvents.GroupBy(x => x.MessageType)
                .Select(x => (x.Key, x.GroupBy(y => y.DestinationAddress?.GetLeftPart(UriPartial.Path))
                    .Select(z => (z.Key, z.ToArray())).ToArray()))
                .ToArray();

            foreach (var (type, destinations) in buckets)
            {
                foreach (var (destinationAddress, events) in destinations)
                {
                    _sendEndpoint.Send<ReportMessageTraffic>(new
                    {
                        MessageTypes = TypeMetadataCache.GetMessageTypes(type),
                        DestinationAddress = destinationAddress,
                        Count = events.Length,
                        MessageIds = events.Select(x => x.MessageId).ToArray()
                    });
                }
            }
        }
    }


    public interface ReportOutboundMessageTraffic
    {
        /// <summary>
        /// The start of this window of traffic
        /// </summary>
        DateTime StartTime { get; }

        /// <summary>
        /// The duration of this traffic window
        /// </summary>
        TimeSpan Duration { get; }

        /// <summary>
        /// The message types supported by this message type
        /// </summary>
        string[] MessageTypes { get; }

        /// <summary>
        /// Where the messages were headed
        /// </summary>
        Uri Destination { get; }

        /// <summary>
        /// THe number of messages sent
        /// </summary>
        int Count { get; }

        /// <summary>
        /// The messages sent
        /// </summary>
        Guid[] MessageIds { get; }

        /// <summary>
        /// A breakdown of the messages in this traffic window
        /// </summary>
        ReportMessageDetail[] Messages { get; }
    }


    public interface ReportMessageDetail
    {
        Guid? MessageId { get; }

        Guid? ConversationId { get; }

        Guid? CorrelationId { get; }

        Guid? InitiatorId { get; }

        Guid? RequestId { get; }

        DateTime SentTime { get; }
    }


    public interface ReportMessageTraffic
    {
        string[] MessageTypes { get; }
        Uri DestinationAddress { get; }
        int Count { get; }
        Guid[] MessageIds { get; }
    }
}