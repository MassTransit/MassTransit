using Amazon.SQS;
using Amazon.SQS.Model;
using MassTransit.Transports;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MassTransit.AmazonSqsTransport
{
    public class MessagesHandler
    {
        public delegate void Action();

        private readonly ClientContext _client;
        private readonly SqsReceiveEndpointContext _context;
        private readonly ReceiveSettings _receiveSettings;
        private readonly IReceivePipeDispatcher _dispatcher;

        public MessagesHandler(ClientContext client, SqsReceiveEndpointContext context, IReceivePipeDispatcher dispatcher)
        {
            _client = client;
            _context = context;
            _dispatcher = dispatcher;

            _receiveSettings = client.GetPayload<ReceiveSettings>();
        }

        public async Task Run(IEnumerable<Message> messages, Action onCompletedCallback)
        {
            if (_receiveSettings.IsOrdered)
            {
                var groupedMessages = GroupMessages(messages);
                foreach (var message in messages.OrderBy(x => x.Attributes.TryGetValue("SequenceNumber", out var sequenceNumber) ? sequenceNumber : "",
                        SequenceNumberComparer.Instance))
                {
                    //With await because it shoud be handled sequently
                    await Handle(message, onCompletedCallback);
                }
            }

            foreach (var message in messages)
            {
                Handle(message, onCompletedCallback);
            }
        }

        private async Task Handle(Message message, Action onCompletedCallback)
        {
            try
            {
                var redelivered = message.Attributes.TryGetInt("ApproximateReceiveCount", out var receiveCount) && receiveCount > 1;

                var context = new AmazonSqsReceiveContext(message, redelivered, _context, _client, _receiveSettings, _client.ConnectionContext);
                try
                {
                    await _dispatcher.Dispatch(context, context).ConfigureAwait(false);
                }
                catch (Exception exception)
                {
                    context.LogTransportFaulted(exception);
                }
                finally
                {
                    context.Dispose();
                }
            }
            finally
            {
                onCompletedCallback();
            }
        }

        private IEnumerable<IGrouping<string, Message>> GroupMessages(IEnumerable<Message> messages)
        {
            return messages.GroupBy(x => x.Attributes.TryGetValue(MessageSystemAttributeName.MessageGroupId, out var groupId) ? groupId : "");
        }
    }

    class SequenceNumberComparer :
            IComparer<string>
    {
        public static readonly SequenceNumberComparer Instance = new SequenceNumberComparer();

        public int Compare(string x, string y)
        {
            if (string.IsNullOrWhiteSpace(x))
                throw new ArgumentNullException(nameof(x));

            if (string.IsNullOrWhiteSpace(y))
                throw new ArgumentNullException(nameof(y));

            if (x.Length != y.Length)
                return x.Length > y.Length ? 1 : -1;

            return string.Compare(x, y, StringComparison.Ordinal);
        }
    }
}
