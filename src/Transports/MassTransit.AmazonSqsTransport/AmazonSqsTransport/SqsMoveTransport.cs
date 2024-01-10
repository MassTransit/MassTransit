namespace MassTransit.AmazonSqsTransport
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Amazon.SQS;
    using Amazon.SQS.Model;
    using Middleware;


    public class SqsMoveTransport<TSettings>
        where TSettings : class
    {
        readonly string _destination;
        readonly bool _isFifo;
        readonly ConfigureAmazonSqsTopologyFilter<TSettings> _topologyFilter;

        protected SqsMoveTransport(string destination, ConfigureAmazonSqsTopologyFilter<TSettings> topologyFilter)
        {
            _destination = destination;
            _topologyFilter = topologyFilter;

            _isFifo = AmazonSqsEndpointAddress.IsFifo(destination);
        }

        protected async Task Move(ReceiveContext context, Action<SendMessageBatchRequestEntry, IDictionary<string, MessageAttributeValue>> preSend)
        {
            if (!context.TryGetPayload(out ClientContext clientContext))
                throw new ArgumentException("The ReceiveContext must contain a ClientContext (from Amazon SQS)", nameof(context));

            OneTimeContext<ConfigureTopologyContext<TSettings>> oneTimeContext = await _topologyFilter.Configure(clientContext).ConfigureAwait(false);

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

            try
            {
                await clientContext.SendMessage(_destination, message, context.CancellationToken).ConfigureAwait(false);
            }
            catch (Exception)
            {
                oneTimeContext.Evict();
                throw;
            }
        }

        static void CopyReceivedMessageHeaders(AmazonSqsMessageContext context, IDictionary<string, MessageAttributeValue> attributes)
        {
            foreach (var key in context.Attributes.Keys.Where(key => !key.StartsWith("MT-")))
                attributes[key] = context.Attributes[key];
        }
    }
}
