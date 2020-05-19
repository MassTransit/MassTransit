namespace MassTransit.AmazonSqsTransport.Transport
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Threading.Tasks;
    using Amazon.SQS.Model;
    using GreenPipes;


    public class SqsMoveTransport
    {
        readonly string _destination;
        readonly IFilter<ClientContext> _topologyFilter;

        protected SqsMoveTransport(string destination, IFilter<ClientContext> topologyFilter)
        {
            _topologyFilter = topologyFilter;
            _destination = destination;
        }

        protected async Task Move(ReceiveContext context, Action<SendMessageBatchRequestEntry, IDictionary<string, MessageAttributeValue>> preSend)
        {
            if (!context.TryGetPayload(out ClientContext clientContext))
                throw new ArgumentException("The ReceiveContext must contain a ClientContext (from Amazon SQS)", nameof(context));

            await _topologyFilter.Send(clientContext, Pipe.Empty<ClientContext>()).ConfigureAwait(false);

            var message = new SendMessageBatchRequestEntry("", Encoding.UTF8.GetString(context.GetBody()));

            CopyReceivedMessageHeaders(context, message.MessageAttributes);

            preSend(message, message.MessageAttributes);

            var task = clientContext.SendMessage(_destination, message, context.CancellationToken);

            context.AddReceiveTask(task);
        }

        static void CopyReceivedMessageHeaders(ReceiveContext context, IDictionary<string, MessageAttributeValue> attributes)
        {
            if (context.TryGetPayload(out AmazonSqsMessageContext messageContext))
            {
                foreach (var key in messageContext.Attributes.Keys)
                {
                    if (key.StartsWith("MT-"))
                        continue;

                    attributes[key] = messageContext.Attributes[key];
                }
            }
        }
    }
}
