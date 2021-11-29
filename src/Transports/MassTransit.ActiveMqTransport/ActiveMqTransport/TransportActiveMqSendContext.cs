namespace MassTransit.ActiveMqTransport
{
    using System.Threading;
    using Apache.NMS;
    using Context;


    public class TransportActiveMqSendContext<T> :
        MessageSendContext<T>,
        ActiveMqSendContext<T>
        where T : class
    {
        public TransportActiveMqSendContext(T message, CancellationToken cancellationToken)
            : base(message, cancellationToken)
        {
        }

        public MsgPriority? Priority { get; set; }
    }
}
