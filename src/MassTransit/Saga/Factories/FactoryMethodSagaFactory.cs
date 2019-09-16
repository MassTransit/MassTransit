namespace MassTransit.Saga.Factories
{
    using System.Threading.Tasks;
    using Context;
    using GreenPipes;


    /// <summary>
    /// Creates a saga instance using the default factory method
    /// </summary>
    /// <typeparam name="TSaga"></typeparam>
    /// <typeparam name="TMessage"></typeparam>
    public class FactoryMethodSagaFactory<TSaga, TMessage> :
        ISagaFactory<TSaga, TMessage>
        where TSaga : class, ISaga
        where TMessage : class
    {
        readonly SagaFactoryMethod<TSaga, TMessage> _factoryMethod;

        public FactoryMethodSagaFactory(SagaFactoryMethod<TSaga, TMessage> factoryMethod)
        {
            _factoryMethod = factoryMethod;
        }

        public TSaga Create(ConsumeContext<TMessage> context)
        {
            if (!context.CorrelationId.HasValue)
                throw new SagaException("The correlationId was not present and the saga could not be created", typeof(TSaga), typeof(TMessage));

            return _factoryMethod(context);
        }

        public Task Send(ConsumeContext<TMessage> context, IPipe<SagaConsumeContext<TSaga, TMessage>> next)
        {
            if (!context.CorrelationId.HasValue)
                throw new SagaException("The correlationId was not present and the saga could not be created", typeof(TSaga), typeof(TMessage));

            TSaga instance = _factoryMethod(context);

            var proxy = new NewSagaConsumeContext<TSaga, TMessage>(context, instance);

            proxy.LogCreated();

            return next.Send(proxy);
        }
    }
}
