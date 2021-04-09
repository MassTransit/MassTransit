using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using GreenPipes;
using MassTransit.Pipeline;

namespace MassTransit.EventStoreDbIntegration
{
    public class ConsumeContextEventStoreDbProducerProvider :
        IEventStoreDbProducerProvider
    {
        readonly ConsumeContext _consumeContext;
        readonly IEventStoreDbProducerProvider _provider;

        public ConsumeContextEventStoreDbProducerProvider(IEventStoreDbProducerProvider provider, ConsumeContext consumeContext)
        {
            _provider = provider;
            _consumeContext = consumeContext;
        }

        public async Task<IEventStoreDbProducer> GetProducer(Uri address)
        {
            var producer = await _provider.GetProducer(address).ConfigureAwait(false);
            return new Producer(producer, _consumeContext);
        }


        class Producer :
            IEventStoreDbProducer
        {
            readonly ConsumeContext _consumeContext;
            readonly IEventStoreDbProducer _producer;

            public Producer(IEventStoreDbProducer producer, ConsumeContext consumeContext)
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
                return Produce(message, Pipe.Empty<EventStoreDbSendContext<T>>(), cancellationToken);
            }

            public Task Produce<T>(IEnumerable<T> messages, CancellationToken cancellationToken = default)
                where T : class
            {
                return Produce(messages, Pipe.Empty<EventStoreDbSendContext<T>>(), cancellationToken);
            }

            public Task Produce<T>(T message, IPipe<EventStoreDbSendContext<T>> pipe, CancellationToken cancellationToken = default)
                where T : class
            {
                var sendPipeAdapter = new ConsumeSendPipeAdapter<T>(pipe, _consumeContext);
                return _producer.Produce(message, sendPipeAdapter, cancellationToken);
            }

            public Task Produce<T>(IEnumerable<T> messages, IPipe<EventStoreDbSendContext<T>> pipe, CancellationToken cancellationToken = default)
                where T : class
            {
                var sendPipeAdapter = new ConsumeSendPipeAdapter<T>(pipe, _consumeContext);
                return _producer.Produce(messages, sendPipeAdapter, cancellationToken);
            }

            public Task Produce<T>(object values, CancellationToken cancellationToken = default)
                where T : class
            {
                return Produce(values, Pipe.Empty<EventStoreDbSendContext<T>>(), cancellationToken);
            }
                
            public Task Produce<T>(IEnumerable<object> values, CancellationToken cancellationToken = default)
                where T : class
            {
                return Produce(values, Pipe.Empty<EventStoreDbSendContext<T>>(), cancellationToken);
            }

            public Task Produce<T>(object values, IPipe<EventStoreDbSendContext<T>> pipe, CancellationToken cancellationToken = default)
                where T : class
            {
                var sendPipeAdapter = new ConsumeSendPipeAdapter<T>(pipe, _consumeContext);
                return _producer.Produce(values, sendPipeAdapter, cancellationToken);
            }

            public Task Produce<T>(IEnumerable<object> values, IPipe<EventStoreDbSendContext<T>> pipe, CancellationToken cancellationToken = default)
                where T : class
            {
                var sendPipeAdapter = new ConsumeSendPipeAdapter<T>(pipe, _consumeContext);
                return _producer.Produce(values, sendPipeAdapter, cancellationToken);
            }
        }


        class ConsumeSendPipeAdapter<T> :
            IPipe<EventStoreDbSendContext<T>>,
            ISendPipe
            where T : class
        {
            readonly ConsumeContext _consumeContext;
            readonly IPipe<EventStoreDbSendContext<T>> _pipe;

            public ConsumeSendPipeAdapter(IPipe<EventStoreDbSendContext<T>> pipe, ConsumeContext consumeContext)
            {
                _pipe = pipe;
                _consumeContext = consumeContext;
            }

            public async Task Send(EventStoreDbSendContext<T> context)
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
