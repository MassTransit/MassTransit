namespace MassTransit.RabbitMqTransport.Topology
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Configuration;


    public class RabbitMqSendSettings :
        RabbitMqExchangeConfigurator,
        SendSettings
    {
        readonly IList<ExchangeBindingPublishTopologySpecification> _exchangeBindings;
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
                SetExchangeArgument("alternate-exchange", address.AlternateExchange);
        }

        public IDictionary<string, object> QueueArguments { get; }

        public RabbitMqEndpointAddress GetSendAddress(Uri hostAddress)
        {
            return new RabbitMqEndpointAddress(hostAddress, ExchangeName, ExchangeType, Durable, AutoDelete, _bindToQueue, _queueName,
                ExchangeArguments.ContainsKey("x-delayed-type") ? (string)ExchangeArguments["x-delayed-type"] : default,
                _exchangeBindings.Count > 0 ? _exchangeBindings.Select(x => x.ExchangeName).ToArray() : default);
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
            var configurator = new RabbitMqExchangeBindingConfigurator(exchangeName, RabbitMQ.Client.ExchangeType.Fanout, Durable, AutoDelete, "");

            configure?.Invoke(configurator);

            _exchangeBindings.Add(new ExchangeBindingPublishTopologySpecification(configurator));
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
