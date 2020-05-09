namespace MassTransit.Containers.Tests.DependencyInjection_Tests
{
    using System;
    using Common_Tests;
    using Microsoft.Extensions.DependencyInjection;
    using NUnit.Framework;
    using Saga;


    [TestFixture]
    public class DependencyInjection_Saga :
        Common_Saga
    {
        readonly IServiceProvider _provider;

        public DependencyInjection_Saga()
        {
            _provider = new ServiceCollection()
                .AddMassTransit(ConfigureRegistration)
                .BuildServiceProvider(true);
        }

        protected override IRegistration Registration => _provider.GetRequiredService<IRegistration>();

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

        protected override IRegistration Registration => _provider.GetRequiredService<IRegistration>();

        protected override ISagaRepository<T> GetSagaRepository<T>()
        {
            return _provider.GetService<ISagaRepository<T>>();
        }
    }
}
