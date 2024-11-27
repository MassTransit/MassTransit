namespace MassTransit.AmazonSqsTransport
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Amazon.SQS.Model;
    using Middleware;
    using Transports;


    public class SqsErrorTransport :
        SqsMoveTransport<ErrorSettings>,
        IErrorTransport
    {
        readonly ITransportSetHeaderAdapter<MessageAttributeValue> _headerAdapter;

        public SqsErrorTransport(string destination, ITransportSetHeaderAdapter<MessageAttributeValue> headerAdapter,
            ConfigureAmazonSqsTopologyFilter<ErrorSettings> topologyFilter)
            : base(destination, topologyFilter)
        {
            _headerAdapter = headerAdapter;
        }

        public Task Send(ExceptionReceiveContext context)
        {
            void PreSend(SendMessageBatchRequestEntry entry, IDictionary<string, MessageAttributeValue> headers)
            {
                _headerAdapter.CopyFrom(headers, context.ExceptionHeaders);
            }

            return Move(context, PreSend);
        }
    }
}
