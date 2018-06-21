namespace MassTransit
{
    using Automatonymous;
    using Automatonymous.Scoping;
    using Lamar;


    public class LamarSagaStateMachineFactory :
        ISagaStateMachineFactory
    {
        readonly IContainer _container;

        public LamarSagaStateMachineFactory(IContainer container)
        {
            _container = container;
        }

        SagaStateMachine<T> ISagaStateMachineFactory.CreateStateMachine<T>()
        {
            return _container.GetInstance<SagaStateMachine<T>>();
        }
    }
}
