namespace MassTransit.RabbitMqTransport.Integration
{
    using System;
    using System.Threading.Tasks;
    using RabbitMQ.Client;
    using Util;


    public class Publish :
        IPendingPublish
    {
        readonly string _exchange;
        readonly string _routingKey;
        readonly IBasicProperties _properties;
        readonly TaskCompletionSource<ulong> _source;

        byte[] _body;
        ulong _publishTag;

        public Publish(string exchange, string routingKey, IBasicProperties properties, byte[] body)
        {
            _exchange = exchange;
            _routingKey = routingKey;
            _properties = properties;
            _body = body;

            _source = TaskUtil.GetTask<ulong>();
        }

        Uri DestinationAddress => string.IsNullOrWhiteSpace(_exchange) ? new Uri($"queue:{_routingKey}") : new Uri($"exchange:{_exchange}");

        public Task Task => _source.Task;

        public int Length => _body.Length;

        public ulong Tag => _publishTag;

        public void Append(IBasicPublishBatch batch, ulong publishTag)
        {
            if (_properties.IsHeadersPresent())
                _properties.Headers["publishId"] = publishTag.ToString("F0");

            batch.Add(_exchange, _routingKey, false, _properties, _body);
            _body = null;

            _publishTag = publishTag;
        }

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

        public void PublishOne(IModel model, ulong publishTag)
        {
            if (_properties.IsHeadersPresent())
                _properties.Headers["publishId"] = publishTag.ToString("F0");

            model.BasicPublish(_exchange, _routingKey, false, _properties, _body);
            _body = null;

            _publishTag = publishTag;
        }
    }
}
