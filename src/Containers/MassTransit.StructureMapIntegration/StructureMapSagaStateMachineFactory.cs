namespace MassTransit.StructureMapIntegration
{
    using Automatonymous;
    using StructureMap;


    public class StructureMapSagaStateMachineFactory :
        ISagaStateMachineFactory
    {
        readonly IContainer _container;

        public StructureMapSagaStateMachineFactory(IContainer container)
        {
            _container = container;
        }

        SagaStateMachine<T> ISagaStateMachineFactory.CreateStateMachine<T>()
        {
            return _container.GetInstance<SagaStateMachine<T>>();
        }
    }
}
