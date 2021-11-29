namespace MassTransit.RabbitMqTransport
{
    using System;
    using System.Threading.Tasks;
    using RabbitMQ.Client;


    public class BatchPublish :
        IPendingConfirmation
    {
        readonly bool _awaitAck;
        readonly TaskCompletionSource<ulong> _confirmed;
        readonly string _exchange;
        readonly IBasicProperties _properties;
        readonly string _routingKey;
        ReadOnlyMemory<byte> _body;

        public BatchPublish(string exchange, string routingKey, IBasicProperties properties, byte[] body, bool awaitAck)
        {
            _exchange = exchange;
            _routingKey = routingKey;
            _properties = properties;
            _body = new ReadOnlyMemory<byte>(body);
            _awaitAck = awaitAck;

            _confirmed = new TaskCompletionSource<ulong>(TaskCreationOptions.RunContinuationsAsynchronously);
        }

        Uri DestinationAddress => string.IsNullOrWhiteSpace(_exchange) ? new Uri($"queue:{_routingKey}") : new Uri($"exchange:{_exchange}");

        public int Length => _body.Length;

        public ulong PublishTag { get; private set; }

        public Task Confirmed => _confirmed.Task;

        public void Acknowledged()
        {
            _confirmed.TrySetResult(PublishTag);
        }

        public void NotAcknowledged()
        {
            _confirmed.TrySetException(new MessageNotAcknowledgedException(DestinationAddress, "The message was not acknowledged by RabbitMQ"));
        }

        public void NotConfirmed(string reason)
        {
            _confirmed.TrySetException(new MessageNotConfirmedException(DestinationAddress, reason));
        }

        public void NotConfirmed(Exception exception)
        {
            _confirmed.TrySetException(new MessageNotConfirmedException(DestinationAddress, exception));
        }

        public void Returned(ushort code, string text)
        {
            _confirmed.TrySetException(new MessageReturnedException(DestinationAddress, $"The message was returned by RabbitMQ: {code}-{text}"));
        }

        public void Append(IBasicPublishBatch batch, ulong publishTag)
        {
            batch.Add(_exchange, _routingKey, false, _properties, _body);
            _body = null;

            PublishTag = publishTag;
        }

        public void BasicPublish(IModel model, ulong publishTag)
        {
            model.BasicPublish(_exchange, _routingKey, false, _properties, _body);
            _body = null;

            PublishTag = publishTag;

            Published();
        }

        public void Published()
        {
            if (_awaitAck && PublishTag > 0)
                return;

            _confirmed.TrySetResult(PublishTag);
        }

        public void SetPublishTag(ulong publishTag)
        {
            PublishTag = publishTag;

            if (_properties.Headers != null)
                _properties.Headers["publishId"] = publishTag.ToString("F0");
        }
    }
}
