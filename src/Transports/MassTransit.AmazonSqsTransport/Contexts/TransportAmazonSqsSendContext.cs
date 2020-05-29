namespace MassTransit.AmazonSqsTransport.Contexts
{
    using System.Threading;
    using Context;


    public class TransportAmazonSqsSendContext<T> :
        MessageSendContext<T>,
        AmazonSqsSendContext<T>
        where T : class
    {
        public TransportAmazonSqsSendContext(T message, CancellationToken cancellationToken)
            : base(message, cancellationToken)
        {
        }

        public string GroupId { get; set; }
        public string DeduplicationId { get; set; }
        public int? DelaySeconds { get; set; }
    }
}
