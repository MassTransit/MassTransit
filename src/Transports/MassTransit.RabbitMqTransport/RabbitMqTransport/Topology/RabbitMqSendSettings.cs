namespace MassTransit.RabbitMqTransport.Topology
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Configuration;
    using RabbitMQ.Client;


    public class RabbitMqSendSettings :
        RabbitMqExchangeConfigurator,
        SendSettings
    {
        readonly List<ExchangeBindingPublishTopologySpecification> _exchangeBindings;
        bool _bindToQueue;
        string _queueName;

        public RabbitMqSendSettings(RabbitMqEndpointAddress address)
            : base(address.Name, address.ExchangeType, address.Durable, address.AutoDelete)
        {
            _exchangeBindings = new List<ExchangeBindingPublishTopologySpecification>();

            QueueArguments = new Dictionary<string, object>();

            if (address.BindToQueue)
                BindToQueue(address.QueueName);

            if (!string.IsNullOrWhiteSpace(address.DelayedType))
                SetExchangeArgument("x-delayed-type", address.DelayedType);

            if (address.BindExchanges != null)
            {
                foreach (var exchange in address.BindExchanges)
                    BindToExchange(exchange);
            }

            if (!string.IsNullOrWhiteSpace(address.AlternateExchange))
                SetExchangeArgument(Headers.AlternateExchange, address.AlternateExchange);

            if (address.SingleActiveConsumer)
                SetQueueArgument(Headers.XSingleActiveConsumer, true);
        }

        public IDictionary<string, object> QueueArguments { get; }

        public RabbitMqEndpointAddress GetSendAddress(Uri hostAddress)
        {
            return new RabbitMqEndpointAddress(hostAddress, ExchangeName, ExchangeType, Durable, AutoDelete, _bindToQueue, _queueName,
                ExchangeArguments.TryGetValue("x-delayed-type", out var argument) ? (string)argument : default,
                _exchangeBindings.Count > 0 ? _exchangeBindings.Select(x => x.ExchangeName).ToArray() : default,
                alternateExchange: ExchangeArguments.TryGetValue(Headers.AlternateExchange, out argument) ? (string)argument : default);
        }

        public BrokerTopology GetBrokerTopology()
        {
            var builder = new PublishEndpointBrokerTopologyBuilder();

            if (ExchangeName.Equals(RabbitMqExchangeNames.ReplyTo, StringComparison.OrdinalIgnoreCase))
                return builder.BuildBrokerTopology();

            builder.Exchange = builder.ExchangeDeclare(ExchangeName, ExchangeType, Durable, AutoDelete, ExchangeArguments);

            foreach (var specification in _exchangeBindings)
                specification.Apply(builder);

            if (_bindToQueue)
            {
                var queue = builder.QueueDeclare(_queueName ?? ExchangeName, Durable, AutoDelete, false, QueueArguments);

                builder.QueueBind(builder.Exchange, queue, "", new Dictionary<string, object>());
            }

            return builder.BuildBrokerTopology();
        }

        public void BindToQueue(string queueName)
        {
            _bindToQueue = true;
            _queueName = queueName;
        }

        public void BindToExchange(string exchangeName, Action<IRabbitMqExchangeBindingConfigurator> configure = null)
        {
            string exchangeType = ExchangeArguments.TryGetValue("x-delayed-type", out var argument) ? (string)argument : RabbitMQ.Client.ExchangeType.Fanout;
            var specification = new ExchangeBindingPublishTopologySpecification(exchangeName, exchangeType, Durable, AutoDelete);

            configure?.Invoke(specification);

            _exchangeBindings.Add(specification);
        }

        public void BindToExchange(RabbitMqEndpointAddress address)
        {
            string exchangeType = ExchangeArguments.TryGetValue("x-delayed-type", out var argument) ? (string)argument : RabbitMQ.Client.ExchangeType.Fanout;
            var specification = new ExchangeBindingPublishTopologySpecification(address.Name, address.ExchangeType, address.Durable, address.AutoDelete);

            _exchangeBindings.Add(specification);
        }

        public void SetQueueArgument(string key, object value)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            if (value == null)
                QueueArguments.Remove(key);
            else
                QueueArguments[key] = value;
        }

        IEnumerable<string> GetSettingStrings()
        {
            if (Durable)
                yield return "durable";

            if (AutoDelete)
                yield return "auto-delete";

            if (ExchangeType != RabbitMQ.Client.ExchangeType.Fanout)
                yield return ExchangeType;

            if (_bindToQueue)
                yield return $"bind->{_queueName}";

            if (ExchangeArguments != null)
            {
                foreach (KeyValuePair<string, object> argument in ExchangeArguments)
                    yield return $"e:{argument.Key}={argument.Value}";
            }

            if (QueueArguments != null)
            {
                foreach (KeyValuePair<string, object> argument in QueueArguments)
                    yield return $"q:{argument.Key}={argument.Value}";
            }
        }

        public override string ToString()
        {
            return string.Join(", ", GetSettingStrings());
        }
    }
}
