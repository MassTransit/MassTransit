namespace MassTransit.Containers.Tests.Autofac_Tests
{
    using System;
    using System.Threading.Tasks;
    using Autofac;
    using Common_Tests;
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

        protected override IRegistration Registration => _container.Resolve<IRegistration>();
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

        protected override IRegistration Registration => _container.Resolve<IRegistration>();
    }
}
