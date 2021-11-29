namespace MassTransit.Testing.Implementations
{
    using System;
    using System.Threading.Tasks;


    public class TestConsumerFactoryDecorator<TConsumer> :
        IConsumerFactory<TConsumer>
        where TConsumer : class, IConsumer
    {
        readonly IConsumerFactory<TConsumer> _consumerFactory;
        readonly ReceivedMessageList _received;

        public TestConsumerFactoryDecorator(IConsumerFactory<TConsumer> consumerFactory, ReceivedMessageList received)
        {
            _consumerFactory = consumerFactory;
            _received = received;
        }

        public Task Send<TMessage>(ConsumeContext<TMessage> context, IPipe<ConsumerConsumeContext<TConsumer, TMessage>> next)
            where TMessage : class
        {
            return _consumerFactory.Send(context, new TestDecoratorPipe<TMessage>(_received, next));
        }

        void IProbeSite.Probe(ProbeContext context)
        {
            var scope = context.CreateScope("testDecorator");

            _consumerFactory.Probe(scope);
        }


        class TestDecoratorPipe<TMessage> :
            IPipe<ConsumerConsumeContext<TConsumer, TMessage>>
            where TMessage : class
        {
            readonly IPipe<ConsumerConsumeContext<TConsumer, TMessage>> _next;
            readonly ReceivedMessageList _received;

            public TestDecoratorPipe(ReceivedMessageList received, IPipe<ConsumerConsumeContext<TConsumer, TMessage>> next)
            {
                _received = received;
                _next = next;
            }

            void IProbeSite.Probe(ProbeContext context)
            {
                _next.Probe(context);
            }

            public async Task Send(ConsumerConsumeContext<TConsumer, TMessage> context)
            {
                try
                {
                    await _next.Send(context).ConfigureAwait(false);

                    _received.Add(context);
                }
                catch (Exception ex)
                {
                    _received.Add(context, ex);

                    throw;
                }
            }
        }
    }
}
