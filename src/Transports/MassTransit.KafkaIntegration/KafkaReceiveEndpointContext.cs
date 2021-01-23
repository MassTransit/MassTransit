namespace MassTransit.KafkaIntegration
{
    using System;
    using Configuration;
    using Confluent.Kafka;
    using Context;
    using Contexts;
    using Exceptions;
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
        readonly Recycle<IConsumerContextSupervisor<TKey, TValue>> _consumerContext;
        readonly ReceiveSettings _settings;

        public KafkaReceiveEndpointContext(IBusInstance busInstance, IReceiveEndpointConfiguration endpointConfiguration,
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
