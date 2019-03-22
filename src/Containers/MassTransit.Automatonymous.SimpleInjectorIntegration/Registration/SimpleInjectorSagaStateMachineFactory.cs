namespace MassTransit.AutomatonymousSimpleInjectorIntegration.Registration
{
    using Automatonymous;
    using Automatonymous.Registration;
    using SimpleInjector;


    public class SimpleInjectorSagaStateMachineFactory :
        ISagaStateMachineFactory
    {
        readonly Container _container;

        public SimpleInjectorSagaStateMachineFactory(Container container)
        {
            _container = container;
        }

        SagaStateMachine<T> ISagaStateMachineFactory.CreateStateMachine<T>()
        {
            return _container.GetInstance<SagaStateMachine<T>>();
        }
    }
}
