namespace MassTransit.AmazonSqsTransport
{
    using System;
    using System.Threading;
    using Context;


    public class AmazonSqsMessageSendContext<T> :
        MessageSendContext<T>,
        AmazonSqsSendContext<T>
        where T : class
    {
        public AmazonSqsMessageSendContext(T message, CancellationToken cancellationToken)
            : base(message, cancellationToken)
        {
        }

        public string GroupId { get; set; }
        public string DeduplicationId { get; set; }

        public int? DelaySeconds
        {
            set => Delay = value.HasValue ? TimeSpan.FromSeconds(value.Value) : default;
        }
    }
}
