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

        protected override IBusRegistrationContext Registration => _provider.GetRequiredService<IBusRegistrationContext>();
    }


    [TestFixture]
    public class DependencyInjection_StateMachine_Filter :
        Common_StateMachine_Filter
    {
        readonly IServiceProvider _provider;

        public DependencyInjection_StateMachine_Filter()
        {
            _provider = new ServiceCollection()
                .AddScoped(_ => new MyId(Guid.NewGuid()))
                .AddSingleton(TaskCompletionSource)
                .AddScoped(typeof(ScopedFilter<>))
                .AddMassTransit(ConfigureRegistration)
                .BuildServiceProvider();
        }

        protected override void ConfigureFilter(IConsumePipeConfigurator configurator)
        {
            DependencyInjectionFilterExtensions.UseConsumeFilter(configurator, typeof(ScopedFilter<>), Registration);
        }

        protected override IBusRegistrationContext Registration => _provider.GetRequiredService<IBusRegistrationContext>();
    }
}
