namespace MassTransit.Containers.Tests.DependencyInjection_Tests
{
    using System;
    using Common_Tests;
    using Microsoft.Extensions.DependencyInjection;
    using NUnit.Framework;
    using TestFramework.Sagas;


    [TestFixture]
    public class DependencyInjection_SagaStateMachine :
        Common_SagaStateMachine
    {
        readonly IServiceProvider _provider;

        public DependencyInjection_SagaStateMachine()
        {
            _provider = new ServiceCollection()
                .AddMassTransit(ConfigureRegistration)
                .AddScoped<PublishTestStartedActivity>()
                .BuildServiceProvider(true);
        }

        protected override IRegistration Registration => _provider.GetRequiredService<IRegistration>();
    }
}
