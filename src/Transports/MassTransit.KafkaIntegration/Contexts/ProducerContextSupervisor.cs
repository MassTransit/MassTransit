namespace MassTransit.KafkaIntegration.Contexts
{
    using System;
    using Configuration;
    using Confluent.Kafka;
    using Context;
    using MassTransit.Pipeline;
    using MassTransit.Pipeline.Observables;
    using Serializers;
    using Transport;
    using Transports;


    public class ProducerContextSupervisor<TKey, TValue> :
        TransportPipeContextSupervisor<ProducerContext<TKey, TValue>>,
        IProducerContextSupervisor<TKey, TValue>
        where TValue : class
    {
        readonly IHostConfiguration _hostConfiguration;
        readonly SendObservable _sendObservers;
        readonly ISendPipe _sendPipe;
        readonly KafkaTopicAddress _topicAddress;

        public ProducerContextSupervisor(string topicName,
            ISendPipe sendPipe, SendObservable sendObservers, IClientContextSupervisor clientContextSupervisor,
            IHostConfiguration hostConfiguration, IHeadersSerializer headersSerializer,
            Func<ProducerBuilder<TKey, TValue>> producerBuilderFactory)
            : base(new ProducerContextFactory<TKey, TValue>(clientContextSupervisor, hostConfiguration, headersSerializer, producerBuilderFactory))
        {
            _sendObservers = sendObservers;
            _hostConfiguration = hostConfiguration;
            _topicAddress = new KafkaTopicAddress(hostConfiguration.HostAddress, topicName);
            _sendPipe = sendPipe;

            clientContextSupervisor.AddSendAgent(this);
        }

        public ITopicProducer<TKey, TValue> CreateProducer()
        {
            var context = new KafkaTransportContext(_sendPipe, _hostConfiguration, _topicAddress);

            if (_sendObservers.Count > 0)
                context.ConnectSendObserver(_sendObservers);

            return new TopicProducer<TKey, TValue>(context, this);
        }


        class KafkaTransportContext :
            BaseSendTransportContext,
            KafkaSendTransportContext
        {
            public KafkaTransportContext(ISendPipe sendPipe, IHostConfiguration hostConfiguration, KafkaTopicAddress topicAddress)
                : base(hostConfiguration)
            {
                SendPipe = sendPipe;
                HostAddress = hostConfiguration.HostAddress;
                TopicAddress = topicAddress;
            }

            public Uri HostAddress { get; }
            public KafkaTopicAddress TopicAddress { get; }
            public ISendPipe SendPipe { get; }
        }
    }
}
