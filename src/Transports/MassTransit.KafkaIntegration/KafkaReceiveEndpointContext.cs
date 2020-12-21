namespace MassTransit.KafkaIntegration
{
    using System;
    using Configuration;
    using Confluent.Kafka;
    using Context;
    using Contexts;
    using GreenPipes;
    using GreenPipes.Agents;
    using MassTransit.Registration;
    using Serializers;
    using Util;


    public class KafkaReceiveEndpointContext<TKey, TValue> :
        BaseReceiveEndpointContext,
        IKafkaReceiveEndpointContext<TKey, TValue>
        where TValue : class
    {
        readonly IBusInstance _busInstance;
        readonly Recycle<IKafkaConsumerContextSupervisor<TKey, TValue>> _consumerContext;
        readonly ReceiveSettings _settings;

        public KafkaReceiveEndpointContext(IBusInstance busInstance, IReceiveEndpointConfiguration endpointConfiguration,
            ReceiveSettings receiveSettings, IHeadersDeserializer headersDeserializer,
            Func<ConsumerBuilder<TKey, TValue>> consumerBuilderFactory)
            : base(busInstance.HostConfiguration, endpointConfiguration)
        {
            _busInstance = busInstance;
            _settings = receiveSettings;

            _consumerContext = new Recycle<IKafkaConsumerContextSupervisor<TKey, TValue>>(() =>
                new KafkaConsumerContextSupervisor<TKey, TValue>(_busInstance.HostConfiguration.Agent, _settings, LogContext, headersDeserializer,
                    consumerBuilderFactory));
        }

        public IKafkaConsumerContextSupervisor<TKey, TValue> ConsumerContextSupervisor => _consumerContext.Supervisor;

        public override void AddAgent(IAgent agent)
        {
            _consumerContext.Supervisor.AddAgent(agent);
        }

        public override Exception ConvertException(Exception exception, string message)
        {
            return exception;
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
