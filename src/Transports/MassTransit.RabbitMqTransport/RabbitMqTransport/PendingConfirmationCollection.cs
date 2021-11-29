namespace MassTransit.RabbitMqTransport
{
    using System.Collections.Concurrent;
    using System.Linq;
    using RabbitMQ.Client;


    public class PendingConfirmationCollection
    {
        readonly ConcurrentDictionary<ulong, IPendingConfirmation> _published;

        public PendingConfirmationCollection()
        {
            _published = new ConcurrentDictionary<ulong, IPendingConfirmation>();
        }

        public IPendingConfirmation Add(string exchange, ulong publishTag, IBasicProperties basicProperties)
        {
            var pendingConfirmation = new PendingConfirmation(exchange, publishTag);
            _published.AddOrUpdate(publishTag, key => pendingConfirmation, (key, existing) =>
            {
                existing.NotConfirmed($"Duplicate key: {key}");

                return pendingConfirmation;
            });

            basicProperties.Headers["publishId"] = publishTag.ToString("F0");

            return pendingConfirmation;
        }

        public void Add(ulong publishTag, BatchPublish batchPublish)
        {
            _published.AddOrUpdate(publishTag, key => batchPublish, (key, existing) =>
            {
                existing.NotConfirmed($"Duplicate key: {key}");

                return batchPublish;
            });

            batchPublish.SetPublishTag(publishTag);
        }

        public void Faulted(IPendingConfirmation pendingConfirmation)
        {
            _published.TryRemove(pendingConfirmation.PublishTag, out _);
        }

        public void Acknowledged(ulong deliveryTag, bool multiple)
        {
            if (multiple)
            {
                foreach (var id in _published.Keys.Where(x => x <= deliveryTag))
                {
                    if (_published.TryRemove(id, out var value))
                        value.Acknowledged();
                }
            }
            else
            {
                if (_published.TryRemove(deliveryTag, out var value))
                    value.Acknowledged();
            }
        }

        public void NotAcknowledged(ulong deliveryTag, bool multiple)
        {
            if (multiple)
            {
                foreach (var id in _published.Keys.Where(x => x <= deliveryTag))
                {
                    if (_published.TryRemove(id, out var value))
                        value.NotAcknowledged();
                }
            }
            else
            {
                if (_published.TryRemove(deliveryTag, out var value))
                    value.NotAcknowledged();
            }
        }

        public void NotConfirmed(string reason)
        {
            foreach (var id in _published.Keys)
            {
                if (_published.TryRemove(id, out var value))
                    value.NotConfirmed(reason);
            }
        }

        public void Returned(ulong deliveryTag, ushort replyCode, string replyText)
        {
            if (_published.TryRemove(deliveryTag, out var published))
                published.Returned(replyCode, replyText);
        }
    }
}
