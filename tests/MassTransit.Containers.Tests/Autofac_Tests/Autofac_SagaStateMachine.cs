namespace MassTransit.Containers.Tests.Autofac_Tests
{
    using System;
    using System.Threading.Tasks;
    using Autofac;
    using Common_Tests;
    using GreenPipes;
    using NUnit.Framework;
    using TestFramework.Sagas;


    [TestFixture]
    public class Autofac_SagaStateMachine :
        Common_SagaStateMachine
    {
        readonly IContainer _container;

        public Autofac_SagaStateMachine()
        {
            var builder = new ContainerBuilder();
            builder.AddMassTransit(ConfigureRegistration);

            builder.RegisterType<PublishTestStartedActivity>();

            _container = builder.Build();
        }

        [OneTimeTearDown]
        public async Task Close_container()
        {
            await _container.DisposeAsync();
        }

        protected override IBusRegistrationContext Registration => _container.Resolve<IBusRegistrationContext>();
    }


    [TestFixture]
    public class Autofac_StateMachine_Filter :
        Common_StateMachine_Filter
    {
        readonly IContainer _container;

        public Autofac_StateMachine_Filter()
        {
            var builder = new ContainerBuilder();
            builder.Register(_ => new MyId(Guid.NewGuid())).InstancePerLifetimeScope();
            builder.RegisterGeneric(typeof(ScopedFilter<>)).InstancePerLifetimeScope();
            builder.RegisterInstance(TaskCompletionSource);
            builder.AddMassTransit(ConfigureRegistration);

            _container = builder.Build();
        }

        [OneTimeTearDown]
        public async Task Close_container()
        {
            await _container.DisposeAsync();
        }

        protected override void ConfigureFilter(IConsumePipeConfigurator configurator)
        {
            AutofacFilterExtensions.UseConsumeFilter(configurator, typeof(ScopedFilter<>), Registration);
        }

        protected override IBusRegistrationContext Registration => _container.Resolve<IBusRegistrationContext>();
    }


    [TestFixture]
    public class Autofac_StateMachine_FilterOrder :
        Common_StateMachine_FilterOrder
    {
        readonly IContainer _container;

        public Autofac_StateMachine_FilterOrder()
        {
            var builder = new ContainerBuilder();
            builder.RegisterInstance(MessageCompletion);
            builder.RegisterInstance(SagaCompletion);
            builder.RegisterInstance(SagaMessageCompletion);
            builder.RegisterType<MessageFilter<StartTest, ILifetimeScope>>().As<IFilter<ConsumeContext<StartTest>>>();
            builder.RegisterType<SagaFilter<TestInstance, ILifetimeScope>>().As<IFilter<SagaConsumeContext<TestInstance>>>();
            builder.RegisterType<SagaMessageFilter<TestInstance, StartTest, ILifetimeScope>>().As<IFilter<SagaConsumeContext<TestInstance, StartTest>>>();
            builder.AddMassTransit(ConfigureRegistration);

            _container = builder.Build();
        }

        [OneTimeTearDown]
        public async Task Close_container()
        {
            await _container.DisposeAsync();
        }

        protected override IBusRegistrationContext Registration => _container.Resolve<IBusRegistrationContext>();

        protected override IFilter<SagaConsumeContext<TestInstance, StartTest>> CreateSagaMessageFilter()
        {
            return _container.Resolve<IFilter<SagaConsumeContext<TestInstance, StartTest>>>();
        }

        protected override IFilter<SagaConsumeContext<TestInstance>> CreateSagaFilter()
        {
            return _container.Resolve<IFilter<SagaConsumeContext<TestInstance>>>();
        }

        protected override IFilter<ConsumeContext<StartTest>> CreateMessageFilter()
        {
            return _container.Resolve<IFilter<ConsumeContext<StartTest>>>();
        }
    }
}
