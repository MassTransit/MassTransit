namespace MassTransit.Containers.Tests.DependencyInjection_Tests
{
    using System;
    using Common_Tests;
    using Microsoft.Extensions.DependencyInjection;
    using NUnit.Framework;
    using Saga;
    using Scenarios;


    [TestFixture]
    public class DependencyInjection_Saga :
        Common_Saga
    {
        readonly IServiceProvider _provider;

        public DependencyInjection_Saga()
        {
            _provider = new ServiceCollection()
                .AddMassTransit(ConfigureRegistration)
                .BuildServiceProvider();
        }

        protected override void ConfigureSaga(IInMemoryReceiveEndpointConfigurator configurator)
        {
            configurator.ConfigureSaga<SimpleSaga>(_provider);
        }

        protected override ISagaRepository<T> GetSagaRepository<T>()
        {
            return _provider.GetService<ISagaRepository<T>>();
        }
    }


    [TestFixture]
    public class DependencyInjection_Saga_Endpoint :
        Common_Saga_Endpoint
    {
        readonly IServiceProvider _provider;

        public DependencyInjection_Saga_Endpoint()
        {
            _provider = new ServiceCollection()
                .AddMassTransit(ConfigureRegistration)
                .BuildServiceProvider();
        }

        protected override void ConfigureEndpoints(IInMemoryBusFactoryConfigurator configurator)
        {
            configurator.ConfigureEndpoints(_provider);
        }

        protected override ISagaRepository<T> GetSagaRepository<T>()
        {
            return _provider.GetService<ISagaRepository<T>>();
        }
    }
}
