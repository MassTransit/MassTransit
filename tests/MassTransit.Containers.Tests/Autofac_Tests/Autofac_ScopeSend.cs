namespace MassTransit.Containers.Tests.Autofac_Tests
{
    using System;
    using System.Threading.Tasks;
    using Autofac;
    using Common_Tests;
    using NUnit.Framework;


    [TestFixture]
    public class Autofac_ScopeSend :
        Common_ScopeSend<ILifetimeScope>
    {
        readonly IContainer _container;
        readonly ILifetimeScope _childContainer;

        public Autofac_ScopeSend()
        {
            var builder = new ContainerBuilder();
            builder.AddMassTransit(x =>
            {
                x.AddBus(provider => BusControl);
            });

            _container = builder.Build();
            _childContainer = _container.BeginLifetimeScope();
        }

        [OneTimeTearDown]
        public async Task Close_container()
        {
            await _childContainer.DisposeAsync();
            await _container.DisposeAsync();
        }

        protected override ISendEndpointProvider GetSendEndpointProvider()
        {
            return _childContainer.Resolve<ISendEndpointProvider>();
        }

        protected override void AssertScopesAreEqual(ILifetimeScope actual)
        {
            Assert.AreEqual(_childContainer, actual);
        }
    }


    [TestFixture]
    public class DependencyInjection_Send_Filter :
        Common_Send_Filter
    {
        readonly IContainer _container;
        readonly ILifetimeScope _scope;

        public DependencyInjection_Send_Filter()
        {
            var builder = new ContainerBuilder();
            builder.Register(_ => new MyId(Guid.NewGuid())).InstancePerLifetimeScope();
            builder.RegisterInstance(TaskCompletionSource);

            builder.AddMassTransit(ConfigureRegistration);

            _container = builder.Build();
            _scope = _container.BeginLifetimeScope();
        }

        [OneTimeTearDown]
        public async Task Close_container()
        {
            await _scope.DisposeAsync();
            await _container.DisposeAsync();
        }

        protected override void ConfigureFilter(ISendPipelineConfigurator configurator)
        {
            AutofacFilterExtensions.UseSendFilter(configurator, typeof(ScopedFilter<>), Registration);
        }

        protected override IBusRegistrationContext Registration => _container.Resolve<IBusRegistrationContext>();
        protected override MyId MyId => _scope.Resolve<MyId>();
        protected override ISendEndpointProvider SendEndpointProvider => _scope.Resolve<ISendEndpointProvider>();
    }
}
