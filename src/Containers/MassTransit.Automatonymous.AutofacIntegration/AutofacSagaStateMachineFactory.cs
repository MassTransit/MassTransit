namespace MassTransit.AutomatonymousAutofacIntegration
{
    using Autofac;
    using Automatonymous;
    using Automatonymous.Scoping;


    public class AutofacSagaStateMachineFactory :
        ISagaStateMachineFactory
    {
        readonly ILifetimeScope _lifetimeScope;

        public AutofacSagaStateMachineFactory(ILifetimeScope lifetimeScope)
        {
            _lifetimeScope = lifetimeScope;
        }

        SagaStateMachine<T> ISagaStateMachineFactory.CreateStateMachine<T>()
        {
            return _lifetimeScope.Resolve<SagaStateMachine<T>>();
        }
    }
}