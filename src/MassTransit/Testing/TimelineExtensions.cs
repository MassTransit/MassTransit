namespace MassTransit.Testing
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Transports;
    using Util;


    public static class TimelineExtensions
    {
        /// <summary>
        /// Output a timeline of messages published, sent, and consumed by the test harness.
        /// </summary>
        /// <param name="harness"></param>
        /// <param name="textWriter"></param>
        /// <param name="configure">Configure the timeout output options</param>
        /// <exception cref="ArgumentNullException"></exception>
        public static async Task OutputTimeline(this IBaseTestHarness harness, TextWriter textWriter, Action<OutputTimelineOptions> configure = default)
        {
            if (harness == null)
                throw new ArgumentNullException(nameof(harness));
            if (textWriter == null)
                throw new ArgumentNullException(nameof(textWriter));

            var options = new OutputTimelineOptions();
            configure?.Invoke(options);

            options.Apply(harness);

            await harness.InactivityTask.ConfigureAwait(false);

            var produced = new List<Message>();

            await foreach (var message in harness.Published.SelectAsync(_ => true).ConfigureAwait(false))
                produced.Add(new Message(message));

            await foreach (var message in harness.Sent.SelectAsync(_ => true).ConfigureAwait(false))
                produced.Add(new Message(message));

            var consumed = new List<Message>();

            await foreach (var message in harness.Consumed.SelectAsync(_ => true).ConfigureAwait(false))
                consumed.Add(new Message(message));

            Dictionary<Guid?, ConversationThread> conversations = produced.GroupBy(m => m.ConversationId).Select(x =>
            {
                List<Message> messages = x.OrderBy(m => m.StartTime).ToList();

                var initiator = messages.FirstOrDefault(m => m.ParentMessageId == default) ?? messages.First();

                var initiatorThread = new ConversationThread(initiator, 1);

                var stack = new Stack<ConversationThread>();
                stack.Push(initiatorThread);

                while (stack.Any())
                {
                    var thread = stack.Pop();

                    List<Message> consumes = consumed.Where(message => message.MessageId == thread.Message.MessageId).ToList();
                    thread.Consumers.AddRange(consumes.Select(message => new ConversationConsumer(message)));

                    IEnumerable<Message> threadMessages = x.Where(m => m.ParentMessageId == thread.Message.MessageId);
                    foreach (var message in threadMessages)
                    {
                        var nextThread = new ConversationThread(message, thread.Depth + 1);
                        thread.Nodes.Add(nextThread);
                        stack.Push(nextThread);
                    }
                }

                return initiatorThread;
            }).ToDictionary(x => x.Message.ConversationId);

            var chart = new ChartTable();

            foreach (var conversation in conversations.Values.OrderBy(x => x.Message.StartTime))
            {
                var whitespace = new string(' ', (conversation.Depth - 1) * 2);
                var conversationLine = $"{whitespace}{conversation.Message.EventType} {options.MessageType(conversation.Message)}";

                chart.Add(conversationLine, conversation.Message.StartTime, conversation.Message.ElapsedTime, conversation.Message.Address);

                AddConsumers(conversation, chart, options);

                var stack = new Stack<ConversationThread>(conversation.Nodes.OrderByDescending(x => x.Message.StartTime));
                while (stack.Any())
                {
                    var current = stack.Pop();

                    whitespace = new string(' ', (current.Depth - 1) * 2);
                    var line = $"{whitespace}{current.Message.EventType} {options.MessageType(current.Message)}";

                    chart.Add(line, current.Message.StartTime, current.Message.ElapsedTime, current.Message.Address);

                    AddConsumers(current, chart, options);

                    foreach (var node in current.Nodes.OrderByDescending(x => x.Message.StartTime))
                        stack.Push(node);
                }
            }

            options.GetTable(chart)
                .SetColumn(1, "Duration", typeof(int))
                .SetRightNumberAlignment()
                .OutputTo(textWriter)
                .Write();
        }

        static void AddConsumers(ConversationThread conversation, ChartTable chart, OutputTimelineOptions options)
        {
            foreach (var consumer in conversation.Consumers.OrderBy(x => x.Message.StartTime))
            {
                var sb = new StringBuilder(60);
                if (conversation.Depth > 1)
                    sb.Append(' ', (conversation.Depth - 1) * 2);
                if (conversation.Depth > 0)
                    sb.Append("\x2514 ");

                sb.Append("Consume ");
                sb.Append(options.MessageType(consumer.Message));

                chart.Add(sb.ToString(), consumer.Message.StartTime, consumer.Message.ElapsedTime, consumer.Message.Address);
            }
        }


        public class OutputTimelineOptions
        {
            bool _includeAddress;
            bool _now;
            bool _trim = true;

            /// <summary>
            /// Include the message namespace in the output. By default, it is removed to save space.
            /// </summary>
            /// <returns></returns>
            public OutputTimelineOptions IncludeNamespace()
            {
                _trim = false;
                return this;
            }

            /// <summary>
            /// Include an additional column with the destination or input address
            /// </summary>
            /// <returns></returns>
            public OutputTimelineOptions IncludeAddress()
            {
                _includeAddress = true;
                return this;
            }

            /// <summary>
            /// Forces the inactivity timeout to now so that the chart renders immediately. By default, the
            /// inactivity timeout is awaited.
            /// </summary>
            /// <returns></returns>
            public OutputTimelineOptions Now()
            {
                _now = true;
                return this;
            }

            internal void Apply(IBaseTestHarness harness)
            {
                if (_now)
                    harness.ForceInactive();
            }

            string GetTypeName(Type type)
            {
                var name = type.Name;

                if (type.IsGenericType)
                {
                    var genericTag = name.LastIndexOf('`');
                    if (genericTag >= 0)
                        name = name.Substring(0, genericTag);

                    return $"{name}<{string.Join(",", type.GetGenericArguments().Select(x => GetTypeName(x)))}>";
                }

                return name;
            }

            internal string MessageType(Message message)
            {
                return _trim
                    ? GetTypeName(message.MessageType)
                    : message.ShortTypeName;
            }

            internal TextTable GetTable(ChartTable chart)
            {
                return _includeAddress
                    ? TextTable.Create(chart.GetRows().Select(x => new
                    {
                        Operation = x.Title,
                        x.Duration,
                        x.Timeline,
                        Address = x.GetColumn(0)
                    }))
                    : TextTable.Create(chart.GetRows());
            }
        }


        class ConversationConsumer
        {
            public ConversationConsumer(Message message)
            {
                Message = message;
            }

            public Message Message { get; }
        }


        class ConversationThread
        {
            public ConversationThread(Message message, int depth)
            {
                Message = message;
                Depth = depth;
                Nodes = new List<ConversationThread>();
                Consumers = new List<ConversationConsumer>();
            }

            public Message Message { get; }
            public int Depth { get; }
            public List<ConversationConsumer> Consumers { get; }
            public List<ConversationThread> Nodes { get; }
        }


        class Conversation
        {
            public Conversation(IEnumerable<ConversationThread> threads)
            {
                Threads = new List<ConversationThread>(threads);
            }

            public IList<ConversationThread> Threads { get; }
        }


        internal class Message
        {
            public Message(IPublishedMessage message)
                : this(message.Context)
            {
                MessageType = message.MessageType;
                ShortTypeName = message.ShortTypeName;
                EventType = MessageEventType.Publish;

                StartTime = message.StartTime;
                ElapsedTime = message.ElapsedTime;
            }

            public Message(ISentMessage message)
                : this(message.Context)
            {
                MessageType = message.MessageType;
                ShortTypeName = message.ShortTypeName;
                EventType = MessageEventType.Send;

                StartTime = message.StartTime;
                ElapsedTime = message.ElapsedTime;
            }

            public Message(IReceivedMessage message)
                : this(message.Context)
            {
                MessageType = message.MessageType;
                ShortTypeName = message.ShortTypeName;
                EventType = MessageEventType.Consume;

                StartTime = message.StartTime;
                ElapsedTime = message.ElapsedTime;
            }

            Message(SendContext context)
            {
                MessageId = context.MessageId;
                ConversationId = context.ConversationId;
                CorrelationId = context.CorrelationId;
                InitiatorId = context.InitiatorId;
                RequestId = context.RequestId;

                Address = context.DestinationAddress.GetEndpointName();

                if (context.TryGetPayload(out ConsumeContext consumeContext))
                    ParentMessageId = consumeContext.MessageId;
            }

            Message(ConsumeContext context)
            {
                MessageId = context.MessageId;
                ConversationId = context.ConversationId;
                CorrelationId = context.CorrelationId;
                InitiatorId = context.InitiatorId;
                RequestId = context.RequestId;

                Address = context.ReceiveContext.InputAddress.GetEndpointName();
            }

            public DateTime StartTime { get; }
            public TimeSpan? ElapsedTime { get; }

            public Guid? MessageId { get; }
            public Guid? ConversationId { get; }
            public Guid? CorrelationId { get; }
            public Guid? InitiatorId { get; }
            public Guid? RequestId { get; }
            public Type MessageType { get; }
            public string ShortTypeName { get; }
            public string EventType { get; }

            public Guid? ParentMessageId { get; }
            public string Address { get; }


            public static class MessageEventType
            {
                public const string Consume = "Consume";
                public const string Publish = "Publish";
                public const string Send = "Send";
            }
        }
    }
}
