namespace MassTransit.Containers.Tests.StructureMap_Tests
{
    using Common_Tests;
    using NUnit.Framework;
    using StructureMap;
    using TestFramework.Sagas;


    [TestFixture]
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

        protected override IRegistration Registration => _container.GetInstance<IRegistration>();
    }
}
