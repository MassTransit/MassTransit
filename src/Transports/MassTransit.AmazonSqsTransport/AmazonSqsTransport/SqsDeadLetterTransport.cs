namespace MassTransit.AmazonSqsTransport
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Amazon.SQS.Model;
    using Transports;


    public class SqsDeadLetterTransport :
        SqsMoveTransport,
        IDeadLetterTransport
    {
        readonly TransportSetHeaderAdapter<MessageAttributeValue> _headerAdapter;

        public SqsDeadLetterTransport(string destination, TransportSetHeaderAdapter<MessageAttributeValue> headerAdapter, IFilter<ClientContext> topologyFilter)
            : base(destination, topologyFilter)
        {
            _headerAdapter = headerAdapter;
        }

        public Task Send(ReceiveContext context, string reason)
        {
            void PreSend(SendMessageBatchRequestEntry entry, IDictionary<string, MessageAttributeValue> headers)
            {
                _headerAdapter.Set(headers, MessageHeaders.Reason, reason ?? "Unspecified");
            }

            return Move(context, PreSend);
        }
    }
}
