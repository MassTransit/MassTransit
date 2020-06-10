namespace MassTransit.Containers.Tests.Autofac_Tests
{
    using System.Threading.Tasks;
    using Autofac;
    using Common_Tests;
    using NUnit.Framework;
    using Saga;


    public class Autofac_Saga :
        Common_Saga
    {
        readonly IContainer _container;

        public Autofac_Saga()
        {
            _container = new ContainerBuilder()
                .AddMassTransit(ConfigureRegistration)
                .Build();
        }

        protected override IBusRegistrationContext Registration => _container.Resolve<IBusRegistrationContext>();

        [OneTimeTearDown]
        public async Task Close_container()
        {
            await _container.DisposeAsync();
        }

        protected override ISagaRepository<T> GetSagaRepository<T>()
        {
            return _container.Resolve<ISagaRepository<T>>();
        }
    }


    public class Autofac_Saga_Endpoint :
        Common_Saga_Endpoint
    {
        readonly IContainer _container;

        public Autofac_Saga_Endpoint()
        {
            _container = new ContainerBuilder()
                .AddMassTransit(ConfigureRegistration)
                .Build();
        }

        protected override IBusRegistrationContext Registration => _container.Resolve<IBusRegistrationContext>();

        [OneTimeTearDown]
        public async Task Close_container()
        {
            await _container.DisposeAsync();
        }

        protected override ISagaRepository<T> GetSagaRepository<T>()
        {
            return _container.Resolve<ISagaRepository<T>>();
        }
    }
}
