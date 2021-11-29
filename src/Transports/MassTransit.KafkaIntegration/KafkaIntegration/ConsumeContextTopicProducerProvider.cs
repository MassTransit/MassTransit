namespace MassTransit.KafkaIntegration
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Transports;


    public class ConsumeContextTopicProducerProvider :
        ITopicProducerProvider
    {
        readonly ConsumeContext _consumeContext;
        readonly ITopicProducerProvider _provider;

        public ConsumeContextTopicProducerProvider(ITopicProducerProvider provider, ConsumeContext consumeContext)
        {
            _provider = provider;
            _consumeContext = consumeContext;
        }

        public ITopicProducer<TKey, TValue> GetProducer<TKey, TValue>(Uri address)
            where TValue : class
        {
            ITopicProducer<TKey, TValue> producer = _provider.GetProducer<TKey, TValue>(address);
            return new TopicProducer<TKey, TValue>(producer, _consumeContext);
        }


        class TopicProducer<TKey, TValue> :
            ITopicProducer<TKey, TValue>
            where TValue : class
        {
            readonly ConsumeContext _consumeContext;
            readonly ITopicProducer<TKey, TValue> _producer;

            public TopicProducer(ITopicProducer<TKey, TValue> producer, ConsumeContext consumeContext)
            {
                _producer = producer;
                _consumeContext = consumeContext;
            }

            public ConnectHandle ConnectSendObserver(ISendObserver observer)
            {
                return _producer.ConnectSendObserver(observer);
            }

            public Task Produce(TKey key, TValue value, CancellationToken cancellationToken = default)
            {
                return Produce(key, value, Pipe.Empty<KafkaSendContext<TKey, TValue>>(), cancellationToken);
            }

            public Task Produce(TKey key, TValue value, IPipe<KafkaSendContext<TKey, TValue>> pipe, CancellationToken cancellationToken = default)
            {
                var sendPipeAdapter = new ConsumeSendPipeAdapter(pipe, _consumeContext);

                return _producer.Produce(key, value, sendPipeAdapter, cancellationToken);
            }

            public Task Produce(TKey key, object values, CancellationToken cancellationToken = default)
            {
                return Produce(key, values, Pipe.Empty<KafkaSendContext<TKey, TValue>>(), cancellationToken);
            }

            public Task Produce(TKey key, object values, IPipe<KafkaSendContext<TKey, TValue>> pipe, CancellationToken cancellationToken = default)
            {
                var sendPipeAdapter = new ConsumeSendPipeAdapter(pipe, _consumeContext);

                return _producer.Produce(key, values, sendPipeAdapter, cancellationToken);
            }


            class ConsumeSendPipeAdapter :
                IPipe<KafkaSendContext<TKey, TValue>>,
                ISendPipe
            {
                readonly ConsumeContext _consumeContext;
                readonly IPipe<KafkaSendContext<TKey, TValue>> _pipe;

                public ConsumeSendPipeAdapter(IPipe<KafkaSendContext<TKey, TValue>> pipe, ConsumeContext consumeContext)
                {
                    _pipe = pipe;
                    _consumeContext = consumeContext;
                }

                public async Task Send(KafkaSendContext<TKey, TValue> context)
                {
                    if (_consumeContext != null)
                        context.TransferConsumeContextHeaders(_consumeContext);

                    if (_pipe.IsNotEmpty())
                        await _pipe.Send(context).ConfigureAwait(false);
                }

                public void Probe(ProbeContext context)
                {
                    _pipe.Probe(context);
                }

                public async Task Send<T>(SendContext<T> context)
                    where T : class
                {
                }
            }
        }
    }
}
