namespace MassTransit.RabbitMqTransport.Integration
{
    using System;
    using System.Threading.Tasks;
    using Util;


    /// <summary>
    /// A pending BasicPublish to RabbitMQ, waiting for an ACK/NAK from the broker
    /// </summary>
    public class PendingPublish
    {
        readonly ConnectionContext _connectionContext;
        readonly string _exchange;
        readonly ulong _publishTag;
        readonly TaskCompletionSource<ulong> _source;

        public PendingPublish(ConnectionContext connectionContext, string exchange, ulong publishTag)
        {
            _connectionContext = connectionContext;
            _exchange = exchange;
            _publishTag = publishTag;
            _source = TaskUtil.GetTask<ulong>();
        }

        public Task Task => _source.Task;

        Uri DestinationAddress => _connectionContext.Topology.GetDestinationAddress(_exchange);

        public void Ack()
        {
            _source.TrySetResult(_publishTag);
        }

        public void Nack()
        {
            _source.TrySetException(new MessageNotAcknowledgedException(DestinationAddress, "The message was not acknowledged by RabbitMQ"));
        }

        public void PublishNotConfirmed(string reason)
        {
            _source.TrySetException(new MessageNotConfirmedException(DestinationAddress, reason));
        }

        public void PublishReturned(ushort code, string text)
        {
            _source.TrySetException(new MessageReturnedException(DestinationAddress, $"The message was returned by RabbitMQ: {code}-{text}"));
        }
    }
}
