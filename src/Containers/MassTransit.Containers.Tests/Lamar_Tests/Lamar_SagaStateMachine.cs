namespace MassTransit.Containers.Tests.Lamar_Tests
{
    using Common_Tests;
    using Lamar;
    using Microsoft.Extensions.DependencyInjection;
    using NUnit.Framework;
    using TestFramework.Sagas;


    [TestFixture]
    public class Lamar_SagaStateMachine :
        Common_SagaStateMachine
    {
        readonly IContainer _container;

        public Lamar_SagaStateMachine()
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

        protected override IRegistration Registration => _container.GetRequiredService<IRegistration>();
    }
}
