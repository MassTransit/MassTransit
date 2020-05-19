namespace MassTransit.Containers.Tests.DependencyInjection_Tests
{
    using System;
    using Common_Tests;
    using Microsoft.Extensions.DependencyInjection;
    using NUnit.Framework;


    [TestFixture]
    public class DependencyInjection_ScopeSend :
        Common_ScopeSend<IServiceProvider>
    {
        readonly IServiceProvider _provider;
        readonly IServiceScope _childContainer;

        public DependencyInjection_ScopeSend()
        {
            var collection = new ServiceCollection();
            collection.AddMassTransit(x =>
            {
                x.AddBus(provider => BusControl);
            });

            _provider = collection.BuildServiceProvider(true);
            _childContainer = _provider.CreateScope();
        }

        [OneTimeTearDown]
        public void Close_container()
        {
            _childContainer.Dispose();
        }

        protected override ISendEndpointProvider GetSendEndpointProvider()
        {
            return _childContainer.ServiceProvider.GetRequiredService<ISendEndpointProvider>();
        }

        protected override void AssertScopesAreEqual(IServiceProvider actual)
        {
            Assert.AreEqual(_childContainer.ServiceProvider, actual);
        }
    }


    [TestFixture]
    public class DependencyInjection_Send_Filter :
        Common_Send_Filter
    {
        readonly IServiceProvider _provider;
        readonly IServiceScope _scope;

        public DependencyInjection_Send_Filter()
        {
            var services = new ServiceCollection();
            services.AddScoped(_ => new MyId(Guid.NewGuid()));
            services.AddSingleton(TaskCompletionSource);
            services.AddScoped(typeof(ScopedFilter<>));

            services.AddMassTransit(ConfigureRegistration);

            _provider = services.BuildServiceProvider();
            _scope = _provider.CreateScope();
        }

        [OneTimeTearDown]
        public void Close_container()
        {
            _scope.Dispose();
        }

        protected override void ConfigureFilter(ISendPipelineConfigurator configurator)
        {
            DependencyInjectionFilterExtensions.UseSendFilter(configurator, typeof(ScopedFilter<>), Registration);
        }

        protected override IRegistration Registration => _provider.GetRequiredService<IRegistration>();
        protected override MyId MyId => _scope.ServiceProvider.GetRequiredService<MyId>();
        protected override ISendEndpointProvider SendEndpointProvider => _scope.ServiceProvider.GetRequiredService<ISendEndpointProvider>();
    }
}
