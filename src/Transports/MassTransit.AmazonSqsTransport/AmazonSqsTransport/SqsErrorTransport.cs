namespace MassTransit.AmazonSqsTransport
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Amazon.SQS.Model;
    using Transports;


    public class SqsErrorTransport :
        SqsMoveTransport,
        IErrorTransport
    {
        readonly ITransportSetHeaderAdapter<MessageAttributeValue> _headerAdapter;

        public SqsErrorTransport(string destination, ITransportSetHeaderAdapter<MessageAttributeValue> headerAdapter, IFilter<ClientContext> topologyFilter)
            : base(destination, topologyFilter)
        {
            _headerAdapter = headerAdapter;
        }

        public Task Send(ExceptionReceiveContext context)
        {
            void PreSend(SendMessageBatchRequestEntry entry, IDictionary<string, MessageAttributeValue> headers)
            {
                _headerAdapter.SetExceptionHeaders(headers, context);
            }

            return Move(context, PreSend);
        }
    }
}
