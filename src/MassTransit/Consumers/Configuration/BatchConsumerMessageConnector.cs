namespace MassTransit.Configuration
{
    using System;
    using System.Threading.Tasks;
    using Batching;
    using Internals;
    using Middleware;


    public class BatchConsumerMessageConnector<TConsumer, TMessage> :
        IConsumerMessageConnector<TConsumer>
        where TConsumer : class, IConsumer<Batch<TMessage>>
        where TMessage : class
    {
        public Type MessageType => typeof(TMessage);

        public IConsumerMessageSpecification<TConsumer> CreateConsumerMessageSpecification()
        {
            return new BatchConsumerMessageSpecification<TConsumer, TMessage>();
        }

        public ConnectHandle ConnectConsumer(IConsumePipeConnector consumePipe, IConsumerFactory<TConsumer> consumerFactory,
            IConsumerSpecification<TConsumer> specification)
        {
            var options = specification.Options<BatchOptions>();

            var messageLimit = options.MessageLimit;
            var timeLimit = options.TimeLimit;
            var concurrencyLimit = options.ConcurrencyLimit;

            IConsumerMessageSpecification<TConsumer, Batch<TMessage>> batchMessageSpecification = specification.GetMessageSpecification<Batch<TMessage>>();

            var consumeFilter = new MethodConsumerMessageFilter<TConsumer, Batch<TMessage>>();

            IPipe<ConsumerConsumeContext<TConsumer, Batch<TMessage>>> batchConsumerPipe = batchMessageSpecification.Build(consumeFilter);

            IPipe<ConsumeContext<Batch<TMessage>>> batchMessagePipe = batchMessageSpecification.BuildMessagePipe(x =>
            {
                specification.ConfigureMessagePipe(x);

                x.UseFilter(new ConsumerMessageFilter<TConsumer, Batch<TMessage>>(consumerFactory, batchConsumerPipe));
            });

            IBatchCollector<TMessage> collector = null;
            if (options.GroupKeyProvider == null)
                collector = new BatchCollector<TMessage>(messageLimit, timeLimit, concurrencyLimit, batchMessagePipe);
            else
            {
                if (options.GroupKeyProvider.GetType().ClosesType(typeof(IGroupKeyProvider<,>), out Type[] types))
                {
                    var collectorType = typeof(BatchCollector<,>).MakeGenericType(typeof(TMessage), types[1]);
                    collector = (IBatchCollector<TMessage>)Activator.CreateInstance(collectorType,
                        messageLimit, timeLimit, concurrencyLimit, batchMessagePipe, options.GroupKeyProvider);
                }
                else
                    throw new ConfigurationException("The GroupKeyProvider does not implement IGroupKeyProvider<TMessage,TKey>");
            }

            var factory = new BatchConsumerFactory<TMessage>(messageLimit, timeLimit, collector);

            IConsumerSpecification<BatchConsumer<TMessage>> messageConsumerSpecification =
                ConsumerConnectorCache<BatchConsumer<TMessage>>.Connector.CreateConsumerSpecification<BatchConsumer<TMessage>>();

            IConsumerMessageSpecification<BatchConsumer<TMessage>, TMessage> messageSpecification =
                messageConsumerSpecification.GetMessageSpecification<TMessage>();

            IPipe<ConsumerConsumeContext<BatchConsumer<TMessage>, TMessage>> consumerPipe =
                messageSpecification.Build(new MethodConsumerMessageFilter<BatchConsumer<TMessage>, TMessage>());

            IPipe<ConsumeContext<TMessage>> messagePipe = messageSpecification.BuildMessagePipe(x =>
            {
                x.UseFilter(new ConsumerMessageFilter<BatchConsumer<TMessage>, TMessage>(factory, consumerPipe));
            });

            var handle = consumePipe.ConnectConsumePipe(messagePipe);

            return new BatchConnectHandle(handle, factory);
        }


        class BatchConnectHandle :
            ConnectHandle
        {
            readonly BatchConsumerFactory<TMessage> _factory;
            readonly ConnectHandle _handle;

            public BatchConnectHandle(ConnectHandle handle, BatchConsumerFactory<TMessage> factory)
            {
                _handle = handle;
                _factory = factory;
            }

            public void Dispose()
            {
                Disconnect();
            }

            public void Disconnect()
            {
                _handle.Disconnect();

                async Task DisposeConsumerFactory()
                {
                    await _factory.DisposeAsync().ConfigureAwait(false);
                }

                Task.Run(DisposeConsumerFactory);
            }
        }
    }
}
