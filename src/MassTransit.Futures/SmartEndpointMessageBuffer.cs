namespace MassTransit
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Util;
    using Util.Caching;


    public class SmartEndpointMessageBuffer :
        IMessageBuffer
    {
        readonly ICache<IMessageEvent> _events;
        readonly IIndex<Guid, IMessageEvent> _messageId;

        public SmartEndpointMessageBuffer()
        {
            _events = new GreenCache<IMessageEvent>(10000, TimeSpan.FromMinutes(1), TimeSpan.FromMinutes(60), () => DateTime.UtcNow);
            _messageId = _events.AddIndex("messageId", x => x.MessageId.Value);
        }

        Task IMessageBuffer.Add<T>(IMessageEvent<T> messageEvent)
        {
            _events.Add(messageEvent);

            return TaskUtil.Completed;
        }

        IEnumerable<IMessageEvent> GetEvents()
        {
            return _events.GetAll();
        }
    }


    public class MessageBundler
    {
        ISendEndpoint _sendEndpoint;

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