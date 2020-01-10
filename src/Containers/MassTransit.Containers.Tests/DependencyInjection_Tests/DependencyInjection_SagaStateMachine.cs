namespace MassTransit.Containers.Tests.DependencyInjection_Tests
{
    using System;
    using Common_Tests;
    using Microsoft.Extensions.DependencyInjection;
    using TestFramework.Sagas;


    public class DependencyInjection_SagaStateMachine :
        Common_SagaStateMachine
    {
        readonly IServiceProvider _provider;

        public DependencyInjection_SagaStateMachine()
        {
            _provider = new ServiceCollection()
                .AddMassTransit(ConfigureRegistration)
                .AddScoped<PublishTestStartedActivity>().BuildServiceProvider();
        }

        protected override void ConfigureSagaStateMachine(IInMemoryReceiveEndpointConfigurator configurator)
        {
            configurator.ConfigureSaga<TestInstance>(_provider);
        }
    }
}
