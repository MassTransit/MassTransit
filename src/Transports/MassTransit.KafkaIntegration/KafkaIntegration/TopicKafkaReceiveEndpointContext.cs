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
        readonly Recycle<IConsumerContextSupervisor<TKey, TValue>> _consumerContext;
        readonly ReceiveSettings _settings;

        public TopicKafkaReceiveEndpointContext(IBusInstance busInstance, IReceiveEndpointConfiguration endpointConfiguration,
            IKafkaHostConfiguration hostConfiguration,
            ReceiveSettings receiveSettings,
            IHeadersDeserializer headersDeserializer,
            Func<ConsumerBuilder<TKey, TValue>> consumerBuilderFactory)
            : base(busInstance.HostConfiguration, endpointConfiguration)
        {
            _busInstance = busInstance;
            _settings = receiveSettings;

            _consumerContext = new Recycle<IConsumerContextSupervisor<TKey, TValue>>(() =>
                new ConsumerContextSupervisor<TKey, TValue>(hostConfiguration.ClientContextSupervisor, _settings, busInstance.HostConfiguration,
                    headersDeserializer, consumerBuilderFactory));
        }

        public IConsumerContextSupervisor<TKey, TValue> ConsumerContextSupervisor => _consumerContext.Supervisor;

        public override Exception ConvertException(Exception exception, string message)
        {
            return new KafkaConnectionException(message + _settings.Topic, exception);
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
            context.Add("topic", _settings.Topic);
            context.Set(_settings);
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
