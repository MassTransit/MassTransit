namespace MassTransit.Containers.Tests.DependencyInjection_Tests
{
    using System;
    using Common_Tests;
    using GreenPipes;
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


    [TestFixture]
    public class DependencyInjection_StateMachine_FilterOrder :
        Common_StateMachine_FilterOrder
    {
        readonly IServiceProvider _provider;

        public DependencyInjection_StateMachine_FilterOrder()
        {
            _provider = new ServiceCollection()
                .AddSingleton(MessageCompletion)
                .AddSingleton(SagaCompletion)
                .AddSingleton(SagaMessageCompletion)
                .AddSingleton<IFilter<ConsumeContext<StartTest>>, MessageFilter<StartTest, IServiceProvider>>()
                .AddSingleton<IFilter<SagaConsumeContext<TestInstance>>, SagaFilter<TestInstance, IServiceProvider>>()
                .AddSingleton<IFilter<SagaConsumeContext<TestInstance, StartTest>>, SagaMessageFilter<TestInstance, StartTest, IServiceProvider>>()
                .AddMassTransit(ConfigureRegistration)
                .BuildServiceProvider();
        }

        protected override IBusRegistrationContext Registration => _provider.GetRequiredService<IBusRegistrationContext>();

        protected override IFilter<SagaConsumeContext<TestInstance, StartTest>> CreateSagaMessageFilter()
        {
            return _provider.GetRequiredService<IFilter<SagaConsumeContext<TestInstance, StartTest>>>();
        }

        protected override IFilter<SagaConsumeContext<TestInstance>> CreateSagaFilter()
        {
            return _provider.GetRequiredService<IFilter<SagaConsumeContext<TestInstance>>>();
        }

        protected override IFilter<ConsumeContext<StartTest>> CreateMessageFilter()
        {
            return _provider.GetRequiredService<IFilter<ConsumeContext<StartTest>>>();
        }
    }
}
