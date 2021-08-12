namespace MassTransit.KafkaIntegration.Contexts
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Configuration;
    using Confluent.Kafka;
    using Context;
    using GreenPipes;
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
            var context = new KafkaTransportContext(_sendPipe, _hostConfiguration, _topicAddress, this);

            if (_sendObservers.Count > 0)
                context.ConnectSendObserver(_sendObservers);

            return new TopicProducer<TKey, TValue>(context);
        }


        class KafkaTransportContext :
            BaseSendTransportContext,
            KafkaSendTransportContext<TKey, TValue>
        {
            readonly IHostConfiguration _hostConfiguration;
            readonly IProducerContextSupervisor<TKey, TValue> _supervisor;

            public KafkaTransportContext(ISendPipe sendPipe, IHostConfiguration hostConfiguration, KafkaTopicAddress topicAddress,
                IProducerContextSupervisor<TKey, TValue> supervisor)
                : base(hostConfiguration)
            {
                _hostConfiguration = hostConfiguration;
                _supervisor = supervisor;
                SendPipe = sendPipe;
                TopicAddress = topicAddress;
            }

            public Uri HostAddress => _hostConfiguration.HostAddress;
            public KafkaTopicAddress TopicAddress { get; }
            public ISendPipe SendPipe { get; }

            public Task Send(IPipe<ProducerContext<TKey, TValue>> pipe, CancellationToken cancellationToken)
            {
                return _hostConfiguration.Retry(() => _supervisor.Send(pipe, cancellationToken), _supervisor, cancellationToken);
            }
        }
    }
}
