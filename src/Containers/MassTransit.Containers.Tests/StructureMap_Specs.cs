namespace MassTransit.Containers.Tests
{
    using NUnit.Framework;
    using Saga;
    using Scenarios;
    using StructureMap;


    public class StructureMap_Saga :
        When_registering_a_saga
    {
        [OneTimeTearDown]
        public void Close_container()
        {
            _container.Dispose();
        }

        readonly IContainer _container;

        public StructureMap_Saga()
        {
            _container = new Container(x =>
            {
                x.RegisterInMemorySagaRepository<SimpleSaga>();

                x.AddMassTransit(cfg => cfg.AddSaga<SimpleSaga>());
            });
        }

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            configurator.ConfigureSagas(_container);
        }

        protected override ISagaRepository<T> GetSagaRepository<T>()
        {
            return _container.GetInstance<ISagaRepository<T>>();
        }
    }
}
