namespace MassTransit.Containers.Tests.StructureMap_Tests
{
    using Common_Tests;
    using StructureMap;
    using NUnit.Framework;
    using TestFramework.Sagas;


    public class StructureMap_SagaStateMachine :
        Common_SagaStateMachine
    {
        readonly IContainer _container;

        public StructureMap_SagaStateMachine()
        {
            _container = new Container(registry =>
            {
                registry.AddMassTransit(ConfigureRegistration);

                registry.For<PublishTestStartedActivity>();
            });
        }

        [OneTimeTearDown]
        public void Close_container()
        {
            _container.Dispose();
        }

        protected override void ConfigureSagaStateMachine(IInMemoryReceiveEndpointConfigurator configurator)
        {
            configurator.ConfigureSaga<TestInstance>(_container);
        }
    }
}
