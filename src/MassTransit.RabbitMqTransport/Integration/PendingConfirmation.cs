namespace MassTransit.RabbitMqTransport.Integration
{
    using System;
    using System.Threading.Tasks;
    using Util;


    /// <summary>
    /// A pending BasicPublish to RabbitMQ, waiting for an ACK/NAK from the broker
    /// </summary>
    public class PendingConfirmation :
        IPendingConfirmation
    {
        readonly ConnectionContext _connectionContext;
        readonly string _exchange;
        readonly TaskCompletionSource<ulong> _source;

        public PendingConfirmation(ConnectionContext connectionContext, string exchange, ulong publishTag)
        {
            _connectionContext = connectionContext;
            _exchange = exchange;
            PublishTag = publishTag;
            _source = TaskUtil.GetTask<ulong>();
        }

        public Task Confirmed => _source.Task;

        public ulong PublishTag { get; }

        Uri DestinationAddress => _connectionContext.Topology.GetDestinationAddress(_exchange);

        public void Acknowledged()
        {
            _source.TrySetResult(PublishTag);
        }

        public void NotAcknowledged()
        {
            _source.TrySetException(new MessageNotAcknowledgedException(DestinationAddress, "The message was not acknowledged by RabbitMQ"));
        }

        public void NotConfirmed(string reason)
        {
            _source.TrySetException(new MessageNotConfirmedException(DestinationAddress, reason));
        }

        public void Returned(ushort code, string text)
        {
            _source.TrySetException(new MessageReturnedException(DestinationAddress, $"The message was returned by RabbitMQ: {code}-{text}"));
        }
    }
}
