namespace MassTransit.RabbitMqTransport
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
        readonly string _exchange;
        readonly TaskCompletionSource<ulong> _source;

        public PendingConfirmation(string exchange, ulong publishTag)
        {
            _exchange = exchange;
            PublishTag = publishTag;
            _source = TaskUtil.GetTask<ulong>();
        }

        Uri DestinationAddress => new Uri($"exchange:{_exchange}");

        public Task Confirmed => _source.Task;

        public ulong PublishTag { get; }

        public void Acknowledged()
        {
            _source.TrySetResult(PublishTag);
        }

        public void NotAcknowledged()
        {
            _source.TrySetException(new MessageNotAcknowledgedException(DestinationAddress, "The message was not acknowledged by RabbitMQ"));
        }

        public void NotConfirmed(Exception exception)
        {
            _source.TrySetException(new MessageNotConfirmedException(DestinationAddress, exception));
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
