namespace MassTransit.AmazonSqsTransport
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Amazon.SQS;
    using Amazon.SQS.Model;


    public class SqsMoveTransport
    {
        readonly string _destination;
        readonly bool _isFifo;
        readonly IFilter<ClientContext> _topologyFilter;

        protected SqsMoveTransport(string destination, IFilter<ClientContext> topologyFilter)
        {
            _destination = destination;
            _topologyFilter = topologyFilter;

            _isFifo = AmazonSqsEndpointAddress.IsFifo(destination);
        }

        protected async Task Move(ReceiveContext context, Action<SendMessageBatchRequestEntry, IDictionary<string, MessageAttributeValue>> preSend)
        {
            if (!context.TryGetPayload(out ClientContext clientContext))
                throw new ArgumentException("The ReceiveContext must contain a ClientContext (from Amazon SQS)", nameof(context));

            await _topologyFilter.Send(clientContext, Pipe.Empty<ClientContext>()).ConfigureAwait(false);

            var message = new SendMessageBatchRequestEntry("", Encoding.UTF8.GetString(context.GetBody()));

            if (context.TryGetPayload(out AmazonSqsMessageContext receiveContext))
            {
                if (_isFifo)
                {
                    if (receiveContext.TransportMessage.Attributes.TryGetValue(MessageSystemAttributeName.MessageGroupId, out var messageGroupId)
                        && !string.IsNullOrWhiteSpace(messageGroupId))
                        message.MessageGroupId = messageGroupId;
                    if (receiveContext.TransportMessage.Attributes.TryGetValue(MessageSystemAttributeName.MessageDeduplicationId,
                            out var messageDeduplicationId)
                        && !string.IsNullOrWhiteSpace(messageDeduplicationId))
                        message.MessageDeduplicationId = messageDeduplicationId;
                }

                CopyReceivedMessageHeaders(receiveContext, message.MessageAttributes);
            }

            preSend(message, message.MessageAttributes);

            var task = clientContext.SendMessage(_destination, message, context.CancellationToken);

            context.AddReceiveTask(task);
        }

        static void CopyReceivedMessageHeaders(AmazonSqsMessageContext context, IDictionary<string, MessageAttributeValue> attributes)
        {
            foreach (var key in context.Attributes.Keys.Where(key => !key.StartsWith("MT-")))
                attributes[key] = context.Attributes[key];
        }
    }
}
