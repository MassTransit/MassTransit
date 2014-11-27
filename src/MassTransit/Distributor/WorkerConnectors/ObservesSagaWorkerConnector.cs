namespace MassTransit.Distributor.WorkerConnectors
{
    using System;
    using System.Linq.Expressions;
    using Magnum.Reflection;
    using Saga;
    using Saga.Pipeline;

    public class ObservesSagaWorkerConnector<TSaga, TMessage> :
        SagaWorkerConnectorBase<TSaga, TMessage>
        where TMessage : class
        where TSaga : class, ISaga, Observes<TMessage, TSaga>
    {
        public ObservesSagaWorkerConnector(ISagaRepository<TSaga> sagaRepository)
            : base(sagaRepository)
        {
        }

        protected override ISagaPolicy<TSaga, TMessage> GetPolicy()
        {
            return new ExistingOrIgnoreSagaPolicy<TSaga, TMessage>(x => false);
        }

        protected override ISagaMessageSink<TSaga, TMessage> GetSagaMessageSink(ISagaRepository<TSaga> sagaRepository,
            ISagaPolicy<TSaga, TMessage> policy)
        {
            var instance = (Observes<TMessage, TSaga>)FastActivator<TSaga>.Create(Guid.Empty);

            Expression<Func<TSaga, TMessage, bool>> selector = instance.GetBindExpression();

            throw new NotImplementedException();
        }
    }
}