namespace MassTransit.KafkaIntegration
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Caching;
    using Observables;
    using Transports;
    using Util;


    public class TopicProducerProvider :
        ITopicProducerProvider
    {
        readonly IBusInstance _busInstance;
        readonly ITopicProducerCache<Uri> _cache;
        readonly IKafkaHostConfiguration _hostConfiguration;
        readonly SendObservable _sendObservers;

        public TopicProducerProvider(IBusInstance busInstance, IKafkaHostConfiguration hostConfiguration)
        {
            _busInstance = busInstance;
            _hostConfiguration = hostConfiguration;

            _cache = new TopicProducerCache<Uri>();
            _sendObservers = new SendObservable();
        }

        public ITopicProducer<TKey, TValue> GetProducer<TKey, TValue>(Uri address)
            where TValue : class
        {
            if (address == null)
                throw new ArgumentNullException(nameof(address));

            Task<ITopicProducer<TKey, TValue>> topicProducerTask = _cache.GetProducer(address, CreateProducer<TKey, TValue>);
            return new AsyncTopicProducer<TKey, TValue>(topicProducerTask);
        }

        public ConnectHandle ConnectSendObserver(ISendObserver observer)
        {
            return _sendObservers.Connect(observer);
        }

        ITopicProducer<TKey, TValue> CreateProducer<TKey, TValue>(Uri address)
            where TValue : class
        {
            var topicAddress = NormalizeAddress(_busInstance.HostConfiguration.HostAddress, address);
            KafkaSendTransportContext<TKey, TValue> context = _hostConfiguration.CreateSendTransportContext<TKey, TValue>(_busInstance, topicAddress.Topic);
            return new TopicProducer<TKey, TValue>(context, context.ConnectSendObserver(_sendObservers));
        }

        static KafkaTopicAddress NormalizeAddress(Uri hostAddress, Uri address)
        {
            return new KafkaTopicAddress(hostAddress, address);
        }


        class AsyncTopicProducer<TKey, TValue> :
            ITopicProducer<TKey, TValue>
            where TValue : class
        {
            readonly Task<ITopicProducer<TKey, TValue>> _topicProducerTask;

            public AsyncTopicProducer(Task<ITopicProducer<TKey, TValue>> topicProducerTask)
            {
                _topicProducerTask = topicProducerTask;
            }

            public ConnectHandle ConnectSendObserver(ISendObserver observer)
            {
                ITopicProducer<TKey, TValue> producer = _topicProducerTask.Status == TaskStatus.RanToCompletion
                    ? _topicProducerTask.Result
                    : TaskUtil.Await(() => _topicProducerTask);
                return producer.ConnectSendObserver(observer);
            }

            public Task Produce(TKey key, TValue value, CancellationToken cancellationToken = default)
            {
                return ProduceInternal(cancellationToken, key, value);
            }

            public Task Produce(TKey key, TValue value, IPipe<KafkaSendContext<TKey, TValue>> pipe, CancellationToken cancellationToken = default)
            {
                return ProduceInternal(cancellationToken, key, value, pipe);
            }

            public Task Produce(TKey key, object values, CancellationToken cancellationToken = default)
            {
                return ProduceInternal(cancellationToken, key, values);
            }

            public Task Produce(TKey key, object values, IPipe<KafkaSendContext<TKey, TValue>> pipe, CancellationToken cancellationToken = default)
            {
                return ProduceInternal(cancellationToken, key, values, pipe);
            }

            Task ProduceInternal(CancellationToken cancellationToken, TKey key, TValue value, IPipe<KafkaSendContext<TKey, TValue>> pipe = null)
            {
                pipe ??= Pipe.Empty<KafkaSendContext<TKey, TValue>>();
                if (_topicProducerTask.Status == TaskStatus.RanToCompletion)
                    return _topicProducerTask.Result.Produce(key, value, pipe, cancellationToken);

                async Task ProduceAsync()
                {
                    ITopicProducer<TKey, TValue> producer = await _topicProducerTask.ConfigureAwait(false);
                    await producer.Produce(key, value, pipe, cancellationToken).ConfigureAwait(false);
                }

                return ProduceAsync();
            }

            Task ProduceInternal(CancellationToken cancellationToken, TKey key, object values, IPipe<KafkaSendContext<TKey, TValue>> pipe = null)
            {
                pipe ??= Pipe.Empty<KafkaSendContext<TKey, TValue>>();
                if (_topicProducerTask.Status == TaskStatus.RanToCompletion)
                    return _topicProducerTask.Result.Produce(key, values, pipe, cancellationToken);

                async Task ProduceAsync()
                {
                    ITopicProducer<TKey, TValue> producer = await _topicProducerTask.ConfigureAwait(false);
                    await producer.Produce(key, values, pipe, cancellationToken).ConfigureAwait(false);
                }

                return ProduceAsync();
            }
        }
    }
}
