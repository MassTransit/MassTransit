namespace MassTransit.ExtensionsDependencyInjectionIntegration.Registration
{
    using System;
    using Automatonymous;
    using Microsoft.Extensions.DependencyInjection;


    public class DependencyInjectionSagaStateMachineFactory :
        ISagaStateMachineFactory
    {
        readonly IServiceProvider _serviceProvider;

        public DependencyInjectionSagaStateMachineFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        SagaStateMachine<T> ISagaStateMachineFactory.CreateStateMachine<T>()
        {
            return _serviceProvider.GetRequiredService<SagaStateMachine<T>>();
        }
    }
}
