namespace MassTransit.EventHubIntegration
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Transports;
    using Util;


    public class ConsumeContextEventHubProducerProvider :
        IEventHubProducerProvider
    {
        readonly ConsumeContext _consumeContext;
        readonly IEventHubProducerProvider _provider;

        public ConsumeContextEventHubProducerProvider(IEventHubProducerProvider provider, ConsumeContext consumeContext)
        {
            _provider = provider;
            _consumeContext = consumeContext;
        }

        public Task<IEventHubProducer> GetProducer(Uri address)
        {
            Task<IEventHubProducer> producerTask = _provider.GetProducer(address);
            IEventHubProducer producer = new Producer(producerTask, _consumeContext);
            return Task.FromResult(producer);
        }

        public ConnectHandle ConnectSendObserver(ISendObserver observer)
        {
            return _provider.ConnectSendObserver(observer);
        }


        class Producer :
            IEventHubProducer
        {
            readonly ConsumeContext _consumeContext;
            readonly Task<IEventHubProducer> _producerTask;

            public Producer(Task<IEventHubProducer> producerTask, ConsumeContext consumeContext)
            {
                _producerTask = producerTask;
                _consumeContext = consumeContext;
            }

            public ConnectHandle ConnectSendObserver(ISendObserver observer)
            {
                var producer = _producerTask.Status == TaskStatus.RanToCompletion
                    ? _producerTask.Result
                    : TaskUtil.Await(() => _producerTask);
                return producer.ConnectSendObserver(observer);
            }

            public Task Produce<T>(T message, CancellationToken cancellationToken = default)
                where T : class
            {
                return Produce(message, Pipe.Empty<EventHubSendContext<T>>(), cancellationToken);
            }

            public Task Produce<T>(IEnumerable<T> messages, CancellationToken cancellationToken = default)
                where T : class
            {
                return Produce(messages, Pipe.Empty<EventHubSendContext<T>>(), cancellationToken);
            }

            public Task Produce<T>(T message, IPipe<EventHubSendContext<T>> pipe, CancellationToken cancellationToken = default)
                where T : class
            {
                var sendPipeAdapter = new ConsumeSendPipeAdapter<T>(pipe, _consumeContext);

                if (_producerTask.Status == TaskStatus.RanToCompletion)
                    return _producerTask.Result.Produce(message, sendPipeAdapter, cancellationToken);

                async Task ProduceAsync()
                {
                    var producer = await _producerTask.ConfigureAwait(false);
                    await producer.Produce(message, sendPipeAdapter, cancellationToken);
                }

                return ProduceAsync();
            }

            public Task Produce<T>(IEnumerable<T> messages, IPipe<EventHubSendContext<T>> pipe, CancellationToken cancellationToken = default)
                where T : class
            {
                var sendPipeAdapter = new ConsumeSendPipeAdapter<T>(pipe, _consumeContext);

                if (_producerTask.Status == TaskStatus.RanToCompletion)
                    return _producerTask.Result.Produce(messages, sendPipeAdapter, cancellationToken);

                async Task ProduceAsync()
                {
                    var producer = await _producerTask.ConfigureAwait(false);
                    await producer.Produce(messages, sendPipeAdapter, cancellationToken);
                }

                return ProduceAsync();
            }

            public Task Produce<T>(object values, CancellationToken cancellationToken = default)
                where T : class
            {
                return Produce(values, Pipe.Empty<EventHubSendContext<T>>(), cancellationToken);
            }

            public Task Produce<T>(IEnumerable<object> values, CancellationToken cancellationToken = default)
                where T : class
            {
                return Produce(values, Pipe.Empty<EventHubSendContext<T>>(), cancellationToken);
            }

            public Task Produce<T>(object values, IPipe<EventHubSendContext<T>> pipe, CancellationToken cancellationToken = default)
                where T : class
            {
                var sendPipeAdapter = new ConsumeSendPipeAdapter<T>(pipe, _consumeContext);

                if (_producerTask.Status == TaskStatus.RanToCompletion)
                    return _producerTask.Result.Produce(values, sendPipeAdapter, cancellationToken);

                async Task ProduceAsync()
                {
                    var producer = await _producerTask.ConfigureAwait(false);
                    await producer.Produce(values, sendPipeAdapter, cancellationToken);
                }

                return ProduceAsync();
            }

            public Task Produce<T>(IEnumerable<object> values, IPipe<EventHubSendContext<T>> pipe, CancellationToken cancellationToken = default)
                where T : class
            {
                var sendPipeAdapter = new ConsumeSendPipeAdapter<T>(pipe, _consumeContext);

                if (_producerTask.Status == TaskStatus.RanToCompletion)
                    return _producerTask.Result.Produce(values, sendPipeAdapter, cancellationToken);

                async Task ProduceAsync()
                {
                    var producer = await _producerTask.ConfigureAwait(false);
                    await producer.Produce(values, sendPipeAdapter, cancellationToken);
                }

                return ProduceAsync();
            }
        }


        class ConsumeSendPipeAdapter<T> :
            IPipe<EventHubSendContext<T>>,
            ISendPipe
            where T : class
        {
            readonly ConsumeContext _consumeContext;
            readonly IPipe<EventHubSendContext<T>> _pipe;

            public ConsumeSendPipeAdapter(IPipe<EventHubSendContext<T>> pipe, ConsumeContext consumeContext)
            {
                _pipe = pipe;
                _consumeContext = consumeContext;
            }

            public async Task Send(EventHubSendContext<T> context)
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

            public async Task Send<TMessage>(SendContext<TMessage> context)
                where TMessage : class
            {
            }
        }
    }
}
