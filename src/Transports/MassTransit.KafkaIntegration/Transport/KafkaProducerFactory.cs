namespace MassTransit.KafkaIntegration.Transport
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Configuration;
    using Confluent.Kafka;
    using Context;
    using GreenPipes;
    using GreenPipes.Agents;
    using Pipeline;
    using Pipeline.Observables;
    using Serializers;
    using Util;


    public class KafkaProducerFactory<TKey, TValue> :
        IKafkaProducerFactory<TKey, TValue>
        where TValue : class
    {
        readonly Recycle<KafkaProducerContext> _context;
        readonly KafkaTopicAddress _topicAddress;

        public KafkaProducerFactory(string topicName, ProducerBuilder<TKey, TValue> producerBuilder, IHostConfiguration hostConfiguration,
            ISendPipeConfiguration sendConfiguration, SendObservable sendObservers, IHeadersSerializer headersSerializer)
        {
            _topicAddress = new KafkaTopicAddress(hostConfiguration.HostAddress, topicName);
            _context = new Recycle<KafkaProducerContext>(() =>
                new KafkaProducerContext(producerBuilder, hostConfiguration, sendConfiguration, sendObservers, headersSerializer));
        }

        public Uri TopicAddress => _topicAddress;

        public ITopicProducer<TKey, TValue> CreateProducer(ConsumeContext consumeContext = null)
        {
            return new TopicProducer<TKey, TValue>(_topicAddress, _context.Supervisor, consumeContext);
        }


        class KafkaProducerContext :
            Agent,
            IKafkaProducerContext<TKey, TValue>
        {
            readonly IHostConfiguration _hostConfiguration;
            readonly IProducer<TKey, TValue> _producer;
            readonly ISendPipe _sendPipe;

            public KafkaProducerContext(ProducerBuilder<TKey, TValue> producerBuilder, IHostConfiguration hostConfiguration,
                ISendPipeConfiguration sendConfiguration,
                SendObservable sendObservers, IHeadersSerializer headersSerializer)
            {
                _producer = producerBuilder.Build();
                _hostConfiguration = hostConfiguration;
                _sendPipe = sendConfiguration.CreatePipe();
                SendObservers = sendObservers;
                HeadersSerializer = headersSerializer;

                hostConfiguration.Agent.Completed.ContinueWith(_ => this.Stop(), TaskContinuationOptions.ExecuteSynchronously);
            }

            public Uri HostAddress => _hostConfiguration.HostAddress;
            public ILogContext LogContext => _hostConfiguration.SendLogContext;
            public SendObservable SendObservers { get; }

            public IHeadersSerializer HeadersSerializer { get; }

            public Task Produce(TopicPartition partition, Message<TKey, TValue> message, CancellationToken cancellationToken)
            {
                return _producer.ProduceAsync(partition, message, cancellationToken);
            }

            protected override Task StopAgent(StopContext context)
            {
                var timeout = TimeSpan.FromSeconds(30);
                _producer.Flush(timeout);
                _producer.Dispose();
                return base.StopAgent(context);
            }

            public Task Send<T>(SendContext<T> context)
                where T : class
            {
                return _sendPipe.Send(context);
            }

            public void Probe(ProbeContext context)
            {
                _sendPipe.Probe(context);
            }
        }
    }
}
