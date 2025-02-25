namespace MassTransit.KafkaIntegration
{
    using System;
    using Confluent.Kafka;
    using MassTransit.Configuration;
    using Serializers;
    using Transports;
    using Util;


    public class TopicKafkaReceiveEndpointContext<TKey, TValue> :
        BaseReceiveEndpointContext,
        KafkaReceiveEndpointContext<TKey, TValue>
        where TValue : class
    {
        readonly IBusInstance _busInstance;
        readonly ReceiveSettings _receiveSetting;
        readonly Recycle<IConsumerContextSupervisor> _consumerContext;

        public TopicKafkaReceiveEndpointContext(IBusInstance busInstance, IKafkaHostConfiguration hostConfiguration,
            string groupId, IReceiveEndpointConfiguration endpointConfiguration, ReceiveSettings receiveSetting,
            IHeadersDeserializer headersDeserializer, IDeserializer<TKey> keyDeserializer, IDeserializer<TValue> valueDeserializer,
            ConsumerBuilderFactory consumerBuilderFactory)
            : base(busInstance.HostConfiguration, endpointConfiguration)
        {
            GroupId = groupId;
            HeadersDeserializer = headersDeserializer;
            KeyDeserializer = keyDeserializer;
            ValueDeserializer = valueDeserializer;
            _busInstance = busInstance;
            _receiveSetting = receiveSetting;

            _consumerContext = new Recycle<IConsumerContextSupervisor>(() =>
                new ConsumerContextSupervisor(busInstance.HostConfiguration, hostConfiguration.ClientContextSupervisor, consumerBuilderFactory));
        }

        public string GroupId { get; }
        public IHeadersDeserializer HeadersDeserializer { get; }
        public IDeserializer<TKey> KeyDeserializer { get; }
        public IDeserializer<TValue> ValueDeserializer { get; }
        public IConsumerContextSupervisor ConsumerContextSupervisor => _consumerContext.Supervisor;

        public KafkaTopicAddress GetInputAddress(string topic)
        {
            return new KafkaTopicAddress(_busInstance.HostConfiguration.HostAddress, topic);
        }

        public override Exception ConvertException(Exception exception, string message)
        {
            return new KafkaConnectionException(message + _receiveSetting.Topic, exception);
        }

        public override void AddSendAgent(IAgent agent)
        {
            _consumerContext.Supervisor.AddSendAgent(agent);
        }

        public override void AddConsumeAgent(IAgent agent)
        {
            _consumerContext.Supervisor.AddConsumeAgent(agent);
        }

        public override void Probe(ProbeContext context)
        {
            context.Add("type", "confluent.kafka");
            context.Add("topic", _receiveSetting.Topic);
            context.Set(_receiveSetting);
        }

        protected override ISendTransportProvider CreateSendTransportProvider()
        {
            throw new NotSupportedException();
        }

        protected override IPublishTransportProvider CreatePublishTransportProvider()
        {
            throw new NotSupportedException();
        }

        protected override IPublishEndpointProvider CreatePublishEndpointProvider()
        {
            return _busInstance.Bus;
        }

        protected override ISendEndpointProvider CreateSendEndpointProvider()
        {
            return _busInstance.Bus;
        }
    }
}
