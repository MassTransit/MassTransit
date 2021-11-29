namespace MassTransit.EventHubIntegration
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Transports;


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

        public async Task<IEventHubProducer> GetProducer(Uri address)
        {
            var producer = await _provider.GetProducer(address).ConfigureAwait(false);
            return new Producer(producer, _consumeContext);
        }


        class Producer :
            IEventHubProducer
        {
            readonly ConsumeContext _consumeContext;
            readonly IEventHubProducer _producer;

            public Producer(IEventHubProducer producer, ConsumeContext consumeContext)
            {
                _producer = producer;
                _consumeContext = consumeContext;
            }

            public ConnectHandle ConnectSendObserver(ISendObserver observer)
            {
                return _producer.ConnectSendObserver(observer);
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
                return _producer.Produce(message, sendPipeAdapter, cancellationToken);
            }

            public Task Produce<T>(IEnumerable<T> messages, IPipe<EventHubSendContext<T>> pipe, CancellationToken cancellationToken = default)
                where T : class
            {
                var sendPipeAdapter = new ConsumeSendPipeAdapter<T>(pipe, _consumeContext);

                return _producer.Produce(messages, sendPipeAdapter, cancellationToken);
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

                return _producer.Produce(values, sendPipeAdapter, cancellationToken);
            }

            public Task Produce<T>(IEnumerable<object> values, IPipe<EventHubSendContext<T>> pipe, CancellationToken cancellationToken = default)
                where T : class
            {
                var sendPipeAdapter = new ConsumeSendPipeAdapter<T>(pipe, _consumeContext);

                return _producer.Produce(values, sendPipeAdapter, cancellationToken);
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
