namespace MassTransit.RabbitMqTransport.Topology
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Configuration;


    public class RabbitMqConsumeTopology :
        ConsumeTopology,
        IRabbitMqConsumeTopologyConfigurator
    {
        readonly IMessageTopology _messageTopology;
        readonly IRabbitMqPublishTopology _publishTopology;
        readonly IList<IRabbitMqConsumeTopologySpecification> _specifications;

        public RabbitMqConsumeTopology(IMessageTopology messageTopology, IRabbitMqPublishTopology publishTopology)
            : base(255)
        {
            _messageTopology = messageTopology;
            _publishTopology = publishTopology;
            ExchangeTypeSelector = new FanoutExchangeTypeSelector();

            _specifications = new List<IRabbitMqConsumeTopologySpecification>();
        }

        public IExchangeTypeSelector ExchangeTypeSelector { get; }

        IRabbitMqMessageConsumeTopology<T> IRabbitMqConsumeTopology.GetMessageTopology<T>()
        {
            return base.GetMessageTopology<T>() as IRabbitMqMessageConsumeTopologyConfigurator<T>;
        }

        public void AddSpecification(IRabbitMqConsumeTopologySpecification specification)
        {
            if (specification == null)
                throw new ArgumentNullException(nameof(specification));

            _specifications.Add(specification);
        }

        IRabbitMqMessageConsumeTopologyConfigurator<T> IRabbitMqConsumeTopologyConfigurator.GetMessageTopology<T>()
        {
            return base.GetMessageTopology<T>() as IRabbitMqMessageConsumeTopologyConfigurator<T>;
        }

        public void Apply(IReceiveEndpointBrokerTopologyBuilder builder)
        {
            foreach (var specification in _specifications)
                specification.Apply(builder);

            ForEach<IRabbitMqMessageConsumeTopologyConfigurator>(x => x.Apply(builder));
        }

        public void Bind(string exchangeName, Action<IRabbitMqExchangeBindingConfigurator> configure = null)
        {
            if (string.IsNullOrWhiteSpace(exchangeName))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(exchangeName));

            var exchangeType = ExchangeTypeSelector.DefaultExchangeType;

            var specification = new ExchangeBindingConsumeTopologySpecification(exchangeName, exchangeType);

            configure?.Invoke(specification);

            _specifications.Add(specification);
        }

        public void BindQueue(string exchangeName, string queueName, Action<IRabbitMqQueueBindingConfigurator> configure = null)
        {
            if (string.IsNullOrWhiteSpace(exchangeName))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(exchangeName));

            var exchangeType = ExchangeTypeSelector.DefaultExchangeType;

            var specification = new ExchangeToQueueBindingConsumeTopologySpecification(exchangeName, exchangeType, queueName);

            configure?.Invoke(specification);

            _specifications.Add(specification);
        }

        public override IEnumerable<ValidationResult> Validate()
        {
            return base.Validate().Concat(_specifications.SelectMany(x => x.Validate()));
        }

        protected override IMessageConsumeTopologyConfigurator CreateMessageTopology<T>(Type type)
        {
            var exchangeTypeSelector = new MessageExchangeTypeSelector<T>(ExchangeTypeSelector);

            var messageTopology = new RabbitMqMessageConsumeTopology<T>(_messageTopology.GetMessageTopology<T>(), exchangeTypeSelector,
                _publishTopology.GetMessageTopology<T>());

            OnMessageTopologyCreated(messageTopology);

            return messageTopology;
        }
    }
}
